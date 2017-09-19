using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using WingTipCommon.Generators;
using WingTipCommon.Identity;
using WingTipCommon.Services;
using WingTipIdentityWebApplication.Api.Models;

namespace WingTipIdentityWebApplication.Api.Controllers
{
    /// <summary>
    /// This controller implements actions that are called by a policy that enable users to be migrated from the SQL identity store
    /// to the Azure Active Directory identity store and synchronized between these stores.
    /// </summary>
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private const string DisplayNameClaimType = "name";
        private const string NonceClaimType = "nonce";
        private const string PlayerTagClaimType = "player_tag";
        private const string TermsOfServiceConsentedClaimValue = "2017-03-09";

        private readonly IGraphService _graphService;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly WingTipUserManager _userManager;

        public AccountController(IGraphService graphService, IPasswordGenerator passwordGenerator, WingTipUserManager userManager)
        {
            _graphService = graphService ?? throw new ArgumentNullException(nameof(graphService));
            _passwordGenerator = passwordGenerator ?? throw new ArgumentNullException(nameof(passwordGenerator));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        /// <summary>
        /// This action is called by a policy when a user is activating a local account.
        /// </summary>
        [Authorize(Roles = "Client")]
        [HttpPost]
        [Route("checkNonce")]
        public async Task<IActionResult> CheckNonce([FromForm] AccountCheckNonceRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Nonce))
            {
                return BadRequest();
            }

            var user = await _userManager.FindByNameAsync(request.UserName);

            // If the user does not exist, then return a bad request result.
            if (user == null)
            {
                return BadRequest();
            }

            var userClaims = await _userManager.GetClaimsAsync(user);
            var nonceClaim = userClaims.FirstOrDefault(c => c.Type == NonceClaimType);

            if (nonceClaim != null && request.Nonce == nonceClaim.Value)
            {
                await _userManager.RemoveClaimAsync(user, nonceClaim);
                await _graphService.SetUserActivationStatusAsync(user.UserName, "Activated");
                return Ok();
            }

            var errorResponse = new AccountCheckNonceErrorResponse
            {
                version = "1.0.0",
                status = (int)HttpStatusCode.Conflict,
                userMessage = "The activation link we sent you is old or it has already been used."
            };

            return new ObjectResult(errorResponse)
            {
                StatusCode = (int)HttpStatusCode.Conflict
            };
        }

        /// <summary>
        /// This action is called by a policy when a user is logging in. It creates the user in the Azure Active Directory identity
        /// store if the user does not exist or updates the password for the user in the Azure Active Directory identity store if the
        /// user does exist.
        /// </summary>
        [Authorize(Roles = "Client")]
        [HttpPost]
        [Route("checkpassword")]
        public async Task<IActionResult> CheckPassword([FromForm] AccountCheckPasswordRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest();
            }

            var user = await _userManager.FindByNameAsync(request.UserName);

            // If the user does not exist, then return an empty result.
            if (user == null)
            {
                return NoContent();
            }

            // If the user does exist and they have not been migrated to the Azure Active Directory identity store with the SQL-managed
            // password, then...
            if (user.MigrationStatus == (int)MigrationStatus.New || user.MigrationStatus == (int)MigrationStatus.NotMigrated || user.MigrationStatus == (int)MigrationStatus.MigratedWithoutPassword)
            {
                var checkPasswordResult = await _userManager.CheckPasswordAsync(user, request.Password);

                if (user.MigrationStatus == (int)MigrationStatus.NotMigrated)
                {
                    var userClaims = await _userManager.GetClaimsAsync(user);
                    var displayNameClaim = userClaims.First(c => c.Type == DisplayNameClaimType);
                    var playerTagClaim = userClaims.FirstOrDefault(c => c.Type == PlayerTagClaimType);

                    // Create the user in the Azure Active Directory identity with either the SQL-managed password, if it is valid,
                    // or a generated password, if it is not valid.
                    await _graphService.CreateUserAsync(
                        request.UserName,
                        checkPasswordResult ? request.Password : _passwordGenerator.GeneratePassword(),
                        displayNameClaim.Value,
                        "Activated",
                        playerTagClaim?.Value,
                        TermsOfServiceConsentedClaimValue);

                    user.MigrationStatus = checkPasswordResult ? (int)MigrationStatus.MigratedWithPassword : (int)MigrationStatus.MigratedWithoutPassword;
                    await _userManager.UpdateAsync(user);
                }
                else if (checkPasswordResult && (user.MigrationStatus == (int)MigrationStatus.New || user.MigrationStatus == (int)MigrationStatus.MigratedWithoutPassword))
                {
                    // Update the password for the user in the Azure Active Directory identity store with the SQL-managed password.
                    await _graphService.SetUserPasswordAsync(request.UserName, request.Password);
                    user.MigrationStatus = (int)MigrationStatus.MigratedWithPassword;
                    await _userManager.UpdateAsync(user);
                }
            }

            return Ok(user);
        }

        /// <summary>
        /// This action is called by a policy when a user is registering. It creates the user in the SQL identity store if the user
        /// does not exist or updates the password for the user in the SQL identity store if the user does exist.
        /// </summary>
        [Authorize(Roles = "Client")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] AccountCreateRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.DisplayName))
            {
                return BadRequest();
            }

            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
            {
                user = new WingTipUser
                {
                    MigrationStatus = (int)MigrationStatus.New,
                    UserName = request.UserName
                };

                var displayNameClaim = new IdentityUserClaim<string>
                {
                    ClaimType = DisplayNameClaimType,
                    ClaimValue = request.DisplayName
                };

                user.Claims.Add(displayNameClaim);

                if (!string.IsNullOrEmpty(request.PlayerTag))
                {
                    var playerTagClaim = new IdentityUserClaim<string>
                    {
                        ClaimType = PlayerTagClaimType,
                        ClaimValue = request.PlayerTag
                    };

                    user.Claims.Add(playerTagClaim);
                }

                // If the user does not exist, then creates the user in the SQL identity store with the Azure Active Directory-managed
                // password.
                await _userManager.CreateAsync(user, request.Password);
            }
            else
            {
                // If the user does exist, then update the password for the user in the SQL identity store to the Azure Active Directory-managed
                // password.
                await _userManager.SetPasswordAsync(user, request.Password);
                user.MigrationStatus = (int)MigrationStatus.MigratedWithPassword;
                await _userManager.UpdateAsync(user);
            }

            return Ok(user);
        }

        /// <summary>
        /// This action is called by a policy when a user is recovering a password. It creates the user in the Azure Active Directory
        /// identity store if the user does not exist.
        /// </summary>
        [Authorize(Roles = "Client")]
        [HttpPost]
        [Route("recoverpassword")]
        public async Task<IActionResult> RecoverPassword([FromForm] AccountRecoverPasswordRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserName))
            {
                return BadRequest();
            }

            var user = await _userManager.FindByNameAsync(request.UserName);

            // If the user does not exist, then return an empty result.
            if (user == null)
            {
                return NoContent();
            }

            // If the user does exist and they have not been migrated to the Azure Active Directory identity store, then...
            if (user.MigrationStatus == (int)MigrationStatus.NotMigrated)
            {
                var userClaims = await _userManager.GetClaimsAsync(user);
                var displayNameClaim = userClaims.First(c => c.Type == DisplayNameClaimType);
                var playerTagClaim = userClaims.FirstOrDefault(c => c.Type == PlayerTagClaimType);

                // Create the user in the Azure Active Directory identity with a generated password.
                await _graphService.CreateUserAsync(
                    request.UserName,
                    _passwordGenerator.GeneratePassword(),
                    displayNameClaim.Value,
                    "Activated",
                    playerTagClaim?.Value,
                    TermsOfServiceConsentedClaimValue);

                user.MigrationStatus = (int)MigrationStatus.MigratedWithoutPassword;
                await _userManager.UpdateAsync(user);
            }

            return Ok(user);
        }

        /// <summary>
        /// This action is called by a policy when a user is resetting a password. It updates the password for the user in the SQL identity
        /// store.
        /// </summary>
        [Authorize(Roles = "Client")]
        [HttpPost]
        [Route("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromForm] AccountResetPasswordRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest();
            }

            var user = await _userManager.FindByNameAsync(request.UserName);

            // If the user does not exist, then return an empty result.
            if (user == null)
            {
                return NoContent();
            }

            // Updates the password for the user in the SQL identity store to the Azure Active Directory-managed password.
            await _userManager.SetPasswordAsync(user, request.NewPassword);
            user.MigrationStatus = (int)MigrationStatus.MigratedWithPassword;
            await _userManager.UpdateAsync(user);
            return Ok(user);
        }
    }
}
