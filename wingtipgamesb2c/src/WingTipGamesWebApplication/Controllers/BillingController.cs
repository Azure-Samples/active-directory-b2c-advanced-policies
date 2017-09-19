using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Mvc;
using WingTipGamesWebApplication.Services;
using WingTipGamesWebApplication.ViewModels.Billing;

namespace WingTipGamesWebApplication.Controllers
{
    public class BillingController : Controller
    {
        private readonly IBillingService _billingService;

        public BillingController(IBillingService billingService)
        {
            _billingService = billingService ?? throw new ArgumentNullException(nameof(billingService));
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            // If the user has not stepped up, then execute the "b2c_1a_step_up" policy.
            if (User.Identity.GetPolicy() != Constants.PolicyIds.StepUp)
            {
                var authenticationProperties = new AuthenticationProperties();
                authenticationProperties.Items[Constants.AuthenticationPropertiesKeys.DomainHint] = User.Identity.GetIdentityProvider();
                authenticationProperties.RedirectUri = Url.Action("Index", "Billing");
                await HttpContext.Authentication.ChallengeAsync(Constants.PolicyIds.StepUp, authenticationProperties, ChallengeBehavior.Unauthorized);
                return new EmptyResult();
            }

            var accessToken = await GetAccessTokenAsync();
            var orders = await _billingService.GetOrdersAsync(accessToken);

            var viewModel = new IndexViewModel
            {
                Orders = orders
            };

            return View(viewModel);
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var authenticateInfo = await HttpContext.Authentication.GetAuthenticateInfoAsync(Constants.AuthenticationSchemes.ApplicationCookie);
            return authenticateInfo.Properties.Items[".Token.access_token"];
        }
    }
}
