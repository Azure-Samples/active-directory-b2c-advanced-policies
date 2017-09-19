using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WingTipCommon.Services;
using WingTipGamesWebApplication.Configuration;
using WingTipGamesWebApplication.Repositories;
using WingTipGamesWebApplication.ViewModels.Invitation;

namespace WingTipGamesWebApplication.Controllers
{
    public class InvitationController : Controller
    {
        private static readonly TimeSpan InvitationTokenLifetime = new TimeSpan(1, 0, 0, 0);

        private readonly IGameRepository _gameRepository;
        private readonly InvitationControllerOptions _options;
        private readonly ISmtpService _smtpService;

        public InvitationController(IGameRepository gameRepository, IOptions<InvitationControllerOptions> optionsAccessor, ISmtpService smtpService)
        {
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));

            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }

            _options = optionsAccessor.Value;
            _smtpService = smtpService ?? throw new ArgumentNullException(nameof(smtpService));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (ModelState.IsValid)
            {
                // Send an invitation message containing a redemption link, which includes a HMAC-based signature, to the "Redeem" action.
                // This action validates this redemption link and if it is valid, then it redirects the end user to the "b2c_1a_invitation"
                // policy.
                if (string.Equals(viewModel.RedemptionMethod, "ApplicationLink"))
                {
                    var redeemUrl = GenerateSignedRedeemUrl(viewModel.EmailAddress);
                    _smtpService.SendInvitationEmail(viewModel.EmailAddress, redeemUrl);
                    return View("Created");
                }

                // Send an invitation message containing a redemption link, which includes a signed JWT with the email address of the
                // invited user, to the "b2c_1a_invitation" policy.
                if (string.Equals(viewModel.RedemptionMethod, "PolicyLink"))
                {
                    var authenticationProperties = new AuthenticationProperties();
                    authenticationProperties.Items[Constants.AuthenticationPropertiesKeys.PlayerProfileRegistrationMode] = "Basic";
                    // Set the invitation lifetime to 1 day.
                    authenticationProperties.Items[Constants.AuthenticationPropertiesKeys.PolicyTokenLifetime] = InvitationTokenLifetime.ToString();
                    // The end user might open the redemption link in a different browser or session so disable the cross-site request
                    // forgery (XSRF) logic in the OpenID Connect authentication middleware.
                    authenticationProperties.Items[Constants.AuthenticationPropertiesKeys.SkipCorrelation] = true.ToString();
                    // Set the email address of the invited user.
                    authenticationProperties.Items[Constants.AuthenticationPropertiesKeys.VerifiedEmail] = viewModel.EmailAddress;
                    authenticationProperties.RedirectUri = Url.Action("Redeemed", "Invitation");
                    await HttpContext.Authentication.ChallengeAsync(Constants.PolicyIds.InvitationLink, authenticationProperties);
                    return View("Created");
                }

                throw new InvalidOperationException();
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Redeem(
            [Bind(Prefix = "email")] string emailAddress,
            [Bind(Prefix = "nbf")] long notBefore,
            [Bind(Prefix = "exp")] long expires,
            [Bind(Prefix = "sig")] string signature)
        {
            if (ValidateSignedRedeemUrl(
                emailAddress,
                notBefore,
                expires,
                signature))
            {
                var authenticationProperties = new AuthenticationProperties();
                authenticationProperties.Items[Constants.AuthenticationPropertiesKeys.PlayerProfileRegistrationMode] = "Basic";
                // Set the email address of the invited user.
                authenticationProperties.Items[Constants.AuthenticationPropertiesKeys.VerifiedEmail] = emailAddress;
                authenticationProperties.RedirectUri = Url.Action("Redeemed", "Invitation");
                await HttpContext.Authentication.ChallengeAsync(Constants.PolicyIds.InvitationRedirect, authenticationProperties);
                return new EmptyResult();
            }

            return View("RedeemError");
        }

        [HttpGet]
        public async Task<IActionResult> Redeemed()
        {
            var newReleaseGames = await _gameRepository.GetNewReleaseGamesAsync();

            var viewModel = new RedeemedViewModel
            {
                Game = newReleaseGames.First()
            };

            return View(viewModel);
        }

        private string GenerateRedeemUrl(
            string emailAddress,
            long notBefore,
            long expires,
            string signature)
        {
            var actionContext = new UrlActionContext
            {
                Protocol = "https",
                Controller = "Invitation",
                Action = "Redeem",
                Values = new
                {
                    email = emailAddress,
                    nbf = notBefore,
                    exp = expires,
                    sig = signature
                }
            };

            return Url.Action(actionContext);
        }

        private string GenerateSignedRedeemUrl(string emailAddress)
        {
            var now = DateTime.UtcNow;
            var notBefore = EpochTime.GetIntDate(now);
            var expires = EpochTime.GetIntDate(now.Add(InvitationTokenLifetime));

            using (var hashAlgorithm = new HMACSHA256(Encoding.UTF8.GetBytes(_options.Key)))
            {
                var signature = Convert.ToBase64String(
                    hashAlgorithm.ComputeHash(
                        Encoding.UTF8.GetBytes(
                            GenerateRedeemUrl(
                                emailAddress,
                                notBefore,
                                expires,
                                string.Empty))));

                return GenerateRedeemUrl(
                    emailAddress,
                    notBefore,
                    expires,
                    signature);
            }
        }

        private bool ValidateSignedRedeemUrl(
            string emailAddress,
            long notBefore,
            long expires,
            string thisSignature)
        {
            using (var hashAlgorithm = new HMACSHA256(Encoding.UTF8.GetBytes(_options.Key)))
            {
                var thatSignature = Convert.ToBase64String(
                    hashAlgorithm.ComputeHash(
                        Encoding.UTF8.GetBytes(
                            GenerateRedeemUrl(
                                emailAddress,
                                notBefore,
                                expires,
                                string.Empty))));

                var now = EpochTime.GetIntDate(DateTime.UtcNow);
                return string.Equals(thatSignature, thisSignature) && now >= notBefore && now < expires;
            }
        }
    }
}
