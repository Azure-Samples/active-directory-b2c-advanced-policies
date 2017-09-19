using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace WingTipToysWebApplication.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public async Task<IActionResult> LogIn(string returnUrl)
        {
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = returnUrl
            };

            await HttpContext.Authentication.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, authenticationProperties);
            return new EmptyResult();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("Index", "Home")
            };

            await HttpContext.Authentication.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, authenticationProperties);
            await HttpContext.Authentication.SignOutAsync(Constants.AuthenticationSchemes.ApplicationCookie);
            return new EmptyResult();
        }
    }
}
