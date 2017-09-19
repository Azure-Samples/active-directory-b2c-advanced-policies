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
using WingTipGamesWebApplication.Configuration;
using WingTipGamesWebApplication.Repositories;
using WingTipGamesWebApplication.ViewModels.Activation;

namespace WingTipGamesWebApplication.Controllers
{
    public class ActivationController : Controller
    {
        private readonly IGameRepository _gameRepository;
        private readonly ActivationControllerOptions _options;

        public ActivationController(IGameRepository gameRepository, IOptions<ActivationControllerOptions> optionsAccessor)
        {
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));

            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }

            _options = optionsAccessor.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Redeem(
            [Bind(Prefix = "email")] string emailAddress,
            [Bind(Prefix = "nonce")] string nonce,
            [Bind(Prefix = "nbf")] long notBefore,
            [Bind(Prefix = "exp")] long expires,
            [Bind(Prefix = "sig")] string signature)
        {
            if (ValidateSignedRedeemUrl(
                emailAddress,
                nonce,
                notBefore,
                expires,
                signature))
            {
                var authenticationProperties = new AuthenticationProperties();
                authenticationProperties.Items[Constants.AuthenticationPropertiesKeys.Nonce] = nonce;
                authenticationProperties.Items[Constants.AuthenticationPropertiesKeys.PlayerProfileRegistrationMode] = "Basic";
                authenticationProperties.Items[Constants.AuthenticationPropertiesKeys.VerifiedEmail] = emailAddress;
                authenticationProperties.RedirectUri = Url.Action("Redeemed", "Activation");
                await HttpContext.Authentication.ChallengeAsync(Constants.PolicyIds.Activation, authenticationProperties);
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
                Games = newReleaseGames.Take(5)
            };

            return View(viewModel);
        }

        private string GenerateRedeemUrl(
            string emailAddress,
            string nonce,
            long notBefore,
            long expires,
            string signature)
        {
            var actionContext = new UrlActionContext
            {
                Protocol = "https",
                Controller = "Activation",
                Action = "Redeem",
                Values = new
                {
                    email = emailAddress,
                    nonce = nonce,
                    nbf = notBefore,
                    exp = expires,
                    sig = signature
                }
            };

            return Url.Action(actionContext);
        }

        private bool ValidateSignedRedeemUrl(
            string emailAddress,
            string nonce,
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
                                nonce,
                                notBefore,
                                expires,
                                string.Empty))));

                var now = EpochTime.GetIntDate(DateTime.UtcNow);
                return string.Equals(thatSignature, thisSignature) && now >= notBefore && now < expires;
            }
        }
    }
}
