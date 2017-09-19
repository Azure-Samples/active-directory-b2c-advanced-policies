using System;
using System.Threading.Tasks;
using WingTipGamesWebApplication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace WingTipGamesWebApplication.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IGeolocationService _geolocationService;

        public AccountController(IGeolocationService geolocationService)
        {
            _geolocationService = geolocationService ?? throw new ArgumentNullException(nameof(geolocationService));
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile()
        {
            var authenticationProperties = new AuthenticationProperties();
            var currentLocale = string.Empty;

            if (Request.Cookies.ContainsKey(Constants.CookieNames.CurrentLocale))
            {
                currentLocale = Request.Cookies[Constants.CookieNames.CurrentLocale];
            }

            if (!string.IsNullOrEmpty(currentLocale))
            {
                authenticationProperties.Items[Constants.AuthenticationPropertiesKeys.UILocales] = currentLocale;
            }

            authenticationProperties.Items[Constants.AuthenticationPropertiesKeys.DomainHint] = User.Identity.GetIdentityProvider();
            authenticationProperties.RedirectUri = Url.Action("Index", "Home");
            await HttpContext.Authentication.ChallengeAsync(Constants.PolicyIds.ProfileUpdate, authenticationProperties, ChallengeBehavior.Unauthorized);
            return new EmptyResult();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Link()
        {
            var authenticationProperties = new AuthenticationProperties();
            var currentLocale = string.Empty;

            if (Request.Cookies.ContainsKey(Constants.CookieNames.CurrentLocale))
            {
                currentLocale = Request.Cookies[Constants.CookieNames.CurrentLocale];
            }

            if (!string.IsNullOrEmpty(currentLocale))
            {
                authenticationProperties.Items[Constants.AuthenticationPropertiesKeys.UILocales] = currentLocale;
            }

            authenticationProperties.RedirectUri = Url.Action("Index", "Home");
            await HttpContext.Authentication.ChallengeAsync(Constants.PolicyIds.Link, authenticationProperties, ChallengeBehavior.Unauthorized);
            return new EmptyResult();
        }

        public IActionResult LinkError(string returnUrl)
        {
            return View();
        }

        public IActionResult LoggedIn(string returnUrl)
        {
            if (User.Identity.IsNewUser())
            {
                TempData[Constants.TempDataKeys.IsNewUser] = true;
            }

            return Redirect(returnUrl);
        }

        [AllowAnonymous]
        public async Task<IActionResult> LogIn(
            string domainHint = "*",
            string localeHint = "*",
            string playerProfileRegistrationMode = "?",
            string returnUrl = "/")
        {
            var authenticationProperties = new AuthenticationProperties();

            if (domainHint != "*")
            {
                // If a domain hint has not been passed, then determine where the user is located...
                if (domainHint == "?")
                {
                    var location = await GetLocationAsync();

                    // If the user is located in Canada, then set the domain hint to "amazon.com".
                    if (location == Constants.Locations.Keys.Canada)
                    {
                        domainHint = Constants.Domains.Amazon;
                    }
                    // Otherwise, set the domain hint to "facebook.com".
                    else
                    {
                        domainHint = Constants.Domains.Facebook;
                    }
                }

                authenticationProperties.Items[Constants.AuthenticationPropertiesKeys.DomainHint] = domainHint;

                Response.Cookies.Append(
                    Constants.CookieNames.CurrentLocation,
                    domainHint,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Path = "/",
                        Secure = false
                    });
            }
            else
            {
                Response.Cookies.Delete(Constants.CookieNames.CurrentLocation);
            }

            if (localeHint != "*")
            {
                // If a UI locale hint has not been passed, then determine where the user is located...
                if (localeHint == "?")
                {
                    var location = await GetLocationAsync();

                    // If the user is located in Canada, then set the UI locale hint to "fr".
                    if (location == Constants.Locations.Keys.Canada)
                    {
                        localeHint = Constants.Cultures.Keys.French;
                    }
                    // Otherwise, set the UI locale hint to "en".
                    else
                    {
                        localeHint = Constants.Cultures.Keys.English;
                    }
                }

                authenticationProperties.Items[Constants.AuthenticationPropertiesKeys.UILocales] = localeHint;

                Response.Cookies.Append(
                    Constants.CookieNames.CurrentLocale,
                    localeHint,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Path = "/",
                        Secure = false
                    });
            }
            else
            {
                Response.Cookies.Delete(Constants.CookieNames.CurrentLocale);
            }

            if (playerProfileRegistrationMode != "*")
            {
                // If a player profile registration mode has not been passed, then randomize it.
                if (playerProfileRegistrationMode == "?")
                {
                    var random = new Random();
                    var nextRandom = random.Next(1, 100);
                    playerProfileRegistrationMode = nextRandom <= 50 ? "Basic" : "Full";
                }

                authenticationProperties.Items[Constants.AuthenticationPropertiesKeys.PlayerProfileRegistrationMode] = playerProfileRegistrationMode;
            }

            authenticationProperties.RedirectUri = Url.Action("LoggedIn", "Account", new
            {
                returnUrl
            });

            await HttpContext.Authentication.ChallengeAsync(Constants.PolicyIds.SignUpOrSignIn, authenticationProperties);
            return new EmptyResult();
        }

        [AllowAnonymous]
        public async Task<IActionResult> LogInUsingAppCode()
        {
            var authenticationProperties = new AuthenticationProperties();
            var currentLocale = string.Empty;

            if (Request.Cookies.ContainsKey(Constants.CookieNames.CurrentLocale))
            {
                currentLocale = Request.Cookies[Constants.CookieNames.CurrentLocale];
            }

            if (!string.IsNullOrEmpty(currentLocale))
            {
                authenticationProperties.Items[Constants.AuthenticationPropertiesKeys.UILocales] = currentLocale;
            }

            authenticationProperties.RedirectUri = Url.Action("Index", "Home");
            await HttpContext.Authentication.ChallengeAsync(Constants.PolicyIds.SignInUsingAppCode, authenticationProperties, ChallengeBehavior.Unauthorized);
            return new EmptyResult();
        }

        [AllowAnonymous]
        public async Task<IActionResult> LogInUsingAuthyCode()
        {
            var authenticationProperties = new AuthenticationProperties();
            var currentLocale = string.Empty;

            if (Request.Cookies.ContainsKey(Constants.CookieNames.CurrentLocale))
            {
                currentLocale = Request.Cookies[Constants.CookieNames.CurrentLocale];
            }

            if (!string.IsNullOrEmpty(currentLocale))
            {
                authenticationProperties.Items[Constants.AuthenticationPropertiesKeys.UILocales] = currentLocale;
            }

            authenticationProperties.RedirectUri = Url.Action("Index", "Home");
            await HttpContext.Authentication.ChallengeAsync(Constants.PolicyIds.SignInUsingAuthyCode, authenticationProperties, ChallengeBehavior.Unauthorized);
            return new EmptyResult();
        }

        [AllowAnonymous]
        public async Task<IActionResult> LogInUsingEmailCode()
        {
            var authenticationProperties = new AuthenticationProperties();
            var currentLocale = string.Empty;

            if (Request.Cookies.ContainsKey(Constants.CookieNames.CurrentLocale))
            {
                currentLocale = Request.Cookies[Constants.CookieNames.CurrentLocale];
            }

            if (!string.IsNullOrEmpty(currentLocale))
            {
                authenticationProperties.Items[Constants.AuthenticationPropertiesKeys.UILocales] = currentLocale;
            }

            authenticationProperties.RedirectUri = Url.Action("Index", "Home");
            await HttpContext.Authentication.ChallengeAsync(Constants.PolicyIds.SignInUsingEmailCode, authenticationProperties, ChallengeBehavior.Unauthorized);
            return new EmptyResult();
        }

        [AllowAnonymous]
        public async Task<IActionResult> LogInUsingPhoneCode()
        {
            var authenticationProperties = new AuthenticationProperties();
            var currentLocale = string.Empty;

            if (Request.Cookies.ContainsKey(Constants.CookieNames.CurrentLocale))
            {
                currentLocale = Request.Cookies[Constants.CookieNames.CurrentLocale];
            }

            if (!string.IsNullOrEmpty(currentLocale))
            {
                authenticationProperties.Items[Constants.AuthenticationPropertiesKeys.UILocales] = currentLocale;
            }

            authenticationProperties.RedirectUri = Url.Action("Index", "Home");
            await HttpContext.Authentication.ChallengeAsync(Constants.PolicyIds.SignInUsingPhoneCode, authenticationProperties, ChallengeBehavior.Unauthorized);
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

            var authenticateInfo = await HttpContext.Authentication.GetAuthenticateInfoAsync(Constants.AuthenticationSchemes.ApplicationCookie);
            await HttpContext.Authentication.SignOutAsync(authenticateInfo.Properties.Items[".AuthScheme"], authenticationProperties);
            await HttpContext.Authentication.SignOutAsync(Constants.AuthenticationSchemes.ApplicationCookie);
            return new EmptyResult();
        }

        [AllowAnonymous]
        public async Task<IActionResult> RecoverPassword()
        {
            var authenticationProperties = new AuthenticationProperties();
            var currentLocale = string.Empty;

            if (Request.Cookies.ContainsKey(Constants.CookieNames.CurrentLocale))
            {
                currentLocale = Request.Cookies[Constants.CookieNames.CurrentLocale];
            }

            if (!string.IsNullOrEmpty(currentLocale))
            {
                authenticationProperties.Items[Constants.AuthenticationPropertiesKeys.UILocales] = currentLocale;
            }

            authenticationProperties.RedirectUri = Url.Action("Index", "Home");
            await HttpContext.Authentication.ChallengeAsync(Constants.PolicyIds.PasswordReset, authenticationProperties, ChallengeBehavior.Unauthorized);
            return new EmptyResult();
        }

        [AllowAnonymous]
        public async Task<IActionResult> RegisterUsingAppCode()
        {
            var authenticationProperties = new AuthenticationProperties();
            var currentLocale = string.Empty;

            if (Request.Cookies.ContainsKey(Constants.CookieNames.CurrentLocale))
            {
                currentLocale = Request.Cookies[Constants.CookieNames.CurrentLocale];
            }

            if (!string.IsNullOrEmpty(currentLocale))
            {
                authenticationProperties.Items[Constants.AuthenticationPropertiesKeys.UILocales] = currentLocale;
            }

            authenticationProperties.RedirectUri = Url.Action("Index", "Home");
            await HttpContext.Authentication.ChallengeAsync(Constants.PolicyIds.SignUpUsingAppCode, authenticationProperties, ChallengeBehavior.Unauthorized);
            return new EmptyResult();
        }

        [AllowAnonymous]
        public async Task<IActionResult> RegisterUsingAuthyCode()
        {
            var authenticationProperties = new AuthenticationProperties();
            var currentLocale = string.Empty;

            if (Request.Cookies.ContainsKey(Constants.CookieNames.CurrentLocale))
            {
                currentLocale = Request.Cookies[Constants.CookieNames.CurrentLocale];
            }

            if (!string.IsNullOrEmpty(currentLocale))
            {
                authenticationProperties.Items[Constants.AuthenticationPropertiesKeys.UILocales] = currentLocale;
            }

            authenticationProperties.RedirectUri = Url.Action("Index", "Home");
            await HttpContext.Authentication.ChallengeAsync(Constants.PolicyIds.SignUpUsingAuthyCode, authenticationProperties, ChallengeBehavior.Unauthorized);
            return new EmptyResult();
        }

        private async Task<string> GetLocationAsync()
        {
            var location = await _geolocationService.GetLocationAsync(Request.HttpContext.Connection.RemoteIpAddress.ToString());

            if (location != null)
            {
                return location.CountryCode;
            }

            return string.Empty;
        }
    }
}
