using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace B2CAppMvc.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(string returnUrl)
        {
            return new ChallengeResult("B2C_1_ProfileEditing", Url.Action("Index", "Home"));
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string authenticationType, string returnUrl)
        {
            return new ChallengeResult(authenticationType, returnUrl);
        }

        [AllowAnonymous]
        public ActionResult LogIn(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOut()
        {
            AuthenticationManager.SignOut(User.Identity.GetAuthenticationClassReference(), DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword()
        {
            return new ChallengeResult("B2C_1_PasswordReset", Url.Action("Index", "Home"));
        }

        #region Helpers

        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string authenticationType, string redirectUri)
                : this(authenticationType, redirectUri, null)
            {
            }

            public ChallengeResult(
                string authenticationType,
                string redirectUri,
                string userId)
            {
                AuthenticationType = authenticationType;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string AuthenticationType { get; set; }

            public string RedirectUri { get; set; }

            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var authenticationProperties = new AuthenticationProperties
                {
                    RedirectUri = RedirectUri
                };

                if (UserId != null)
                {
                    authenticationProperties.Dictionary[XsrfKey] = UserId;
                }

                context.HttpContext.GetOwinContext().Authentication.Challenge(authenticationProperties, AuthenticationType);
            }
        }

        #endregion
    }
}
