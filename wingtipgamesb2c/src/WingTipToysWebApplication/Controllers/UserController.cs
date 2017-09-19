using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WingTipCommon;
using WingTipCommon.Generators;
using WingTipCommon.Identity;
using WingTipCommon.Services;
using WingTipToysWebApplication.Configuration;
using WingTipToysWebApplication.ViewModels.User;

namespace WingTipToysWebApplication.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private const string DisplayNameClaimType = "name";
        private const string PlayerTagClaimType = "player_tag";
        private const string TermsOfServiceConsentedClaimValue = "2017-03-09";

        private static readonly TimeSpan ActivationTokenLifetime = new TimeSpan(1, 0, 0, 0);

        private readonly IGraphService _graphService;
        private readonly UserControllerOptions _options;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly ISmtpService _smtpService;
        private readonly WingTipUserManager _userManager;

        public UserController(
            IGraphService graphService,
            IOptions<UserControllerOptions> optionsAccessor,
            IPasswordGenerator passwordGenerator,
            ISmtpService smtpService,
            WingTipUserManager userManager)
        {
            _graphService = graphService ?? throw new ArgumentNullException(nameof(graphService));

            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }

            _options = optionsAccessor.Value;
            _passwordGenerator = passwordGenerator ?? throw new ArgumentNullException(nameof(passwordGenerator));
            _smtpService = smtpService ?? throw new ArgumentNullException(nameof(smtpService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<IActionResult> Activate(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new InvalidOperationException();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                throw new InvalidOperationException();
            }

            if (user.MigrationStatus == (int)MigrationStatus.NotMigrated)
            {
                throw new InvalidOperationException();
            }

            await _graphService.SetUserActivationStatusAsync(user.UserName, "Pending");
            var userClaims = await _userManager.GetClaimsAsync(user);
            var displayNameClaim = userClaims.First(c => c.Type == WingTipClaimTypes.DisplayNameClaimType);
            var nonce = _passwordGenerator.GeneratePassword();
            var redeemUrl = GenerateSignedRedeemUrl(user.UserName, nonce);
            _smtpService.SendActivationEmail(user.UserName, displayNameClaim.Value, redeemUrl);
            var existingNonceClaim = userClaims.FirstOrDefault(c => c.Type == WingTipClaimTypes.NonceClaimType);

            if (existingNonceClaim != null)
            {
                var removeClaimResult = await _userManager.RemoveClaimAsync(user, existingNonceClaim);

                if (!removeClaimResult.Succeeded)
                {
                    throw new InvalidOperationException(removeClaimResult.Errors.ToString());
                }
            }

            var newNonceClaim = new Claim(WingTipClaimTypes.NonceClaimType, nonce);
            var addClaimResult = await _userManager.AddClaimAsync(user, newNonceClaim);

            if (!addClaimResult.Succeeded)
            {
                throw new InvalidOperationException(addClaimResult.Errors.ToString());
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ChangePassword(string id)
        {
            var viewModel = new ChangePasswordViewModel
            {
                Id = id
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (!ModelState.IsValid)
            {
                return new EmptyResult();
            }

            var user = await _userManager.FindByIdAsync(viewModel.Id);

            if (user == null)
            {
                throw new InvalidOperationException();
            }

            await _userManager.SetPasswordAsync(user, viewModel.NewPassword);

            if (user.MigrationStatus == (int)MigrationStatus.NotMigrated)
            {
                var userClaims = await _userManager.GetClaimsAsync(user);
                var displayNameClaim = userClaims.First(c => c.Type == DisplayNameClaimType);
                var playerTagClaim = userClaims.FirstOrDefault(c => c.Type == PlayerTagClaimType);

                await _graphService.CreateUserAsync(
                    user.UserName,
                    viewModel.NewPassword,
                    displayNameClaim.Value,
                    "NotActivated",
                    playerTagClaim?.Value,
                    TermsOfServiceConsentedClaimValue);
            }
            else
            {
                await _graphService.SetUserPasswordAsync(user.UserName, viewModel.NewPassword);
            }

            user.MigrationStatus = (int)MigrationStatus.MigratedWithPassword;
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateViewModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = new WingTipUser
            {
                MigrationStatus = (int)MigrationStatus.NotMigrated,
                UserName = viewModel.UserName
            };

            var displayNameClaim = new IdentityUserClaim<string>
            {
                ClaimType = WingTipClaimTypes.DisplayNameClaimType,
                ClaimValue = viewModel.DisplayName
            };

            user.Claims.Add(displayNameClaim);

            if (!string.IsNullOrEmpty(viewModel.PlayerTag))
            {
                var playerTagClaim = new IdentityUserClaim<string>
                {
                    ClaimType = WingTipClaimTypes.PlayerTagClaimType,
                    ClaimValue = viewModel.PlayerTag
                };

                user.Claims.Add(playerTagClaim);
            }

            var createResult = await _userManager.CreateAsync(user, viewModel.Password);

            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(createResult.Errors.ToString());
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new InvalidOperationException();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                throw new InvalidOperationException();
            }

            await _userManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Demigrate(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new InvalidOperationException();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                throw new InvalidOperationException();
            }

            if (user.MigrationStatus != (int)MigrationStatus.MigratedWithoutPassword && user.MigrationStatus != (int)MigrationStatus.MigratedWithPassword)
            {
                throw new InvalidOperationException();
            }

            await _graphService.DeleteUserAsync(user.UserName);
            user.MigrationStatus = (int)MigrationStatus.NotMigrated;
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();

            var viewModel = new IndexViewModel
            {
                Users = users.Select(user => new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    MigrationStatus = (MigrationStatus)user.MigrationStatus
                }).ToList()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Migrate(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new InvalidOperationException();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                throw new InvalidOperationException();
            }

            if (user.MigrationStatus != (int)MigrationStatus.NotMigrated)
            {
                throw new InvalidOperationException();
            }

            var userClaims = await _userManager.GetClaimsAsync(user);
            var displayNameClaim = userClaims.First(c => c.Type == WingTipClaimTypes.DisplayNameClaimType);
            var playerTagClaim = userClaims.FirstOrDefault(c => c.Type == WingTipClaimTypes.PlayerTagClaimType);

            await _graphService.CreateUserAsync(
                user.UserName,
                _passwordGenerator.GeneratePassword(),
                displayNameClaim.Value,
                "NotActivated",
                playerTagClaim?.Value,
                TermsOfServiceConsentedClaimValue);

            user.MigrationStatus = (int)MigrationStatus.MigratedWithoutPassword;
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Reset(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new InvalidOperationException();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                throw new InvalidOperationException();
            }

            await _graphService.ResetAsync(user.UserName);

            return RedirectToAction("Index");
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
                Host = _options.ActivationHost,
                Controller = _options.ActivationController,
                Action = _options.ActivationAction,
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

        private string GenerateSignedRedeemUrl(string emailAddress, string nonce)
        {
            var now = DateTime.UtcNow;
            var notBefore = EpochTime.GetIntDate(now);
            var expires = EpochTime.GetIntDate(now.Add(ActivationTokenLifetime));

            using (var hashAlgorithm = new HMACSHA256(Encoding.UTF8.GetBytes(_options.ActivationKey)))
            {
                var signature = Convert.ToBase64String(
                    hashAlgorithm.ComputeHash(
                        Encoding.UTF8.GetBytes(GenerateRedeemUrl(
                            emailAddress,
                            nonce,
                            notBefore,
                            expires,
                            string.Empty))));

                return GenerateRedeemUrl(
                    emailAddress,
                    nonce,
                    notBefore,
                    expires,
                    signature);
            }
        }
    }
}
