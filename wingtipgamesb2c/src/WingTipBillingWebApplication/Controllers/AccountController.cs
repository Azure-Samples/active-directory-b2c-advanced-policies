using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas;
using Microsoft.AspNetCore.Mvc;
using WingTipBillingWebApplication.Identity;
using WingTipBillingWebApplication.Identity.Saml2;

namespace WingTipBillingWebApplication.Controllers
{
    public class AccountController : Controller
    {
        private const string ReturnUrlRelayStateKey = "ReturnUrl";

        private readonly Saml2Configuration _configuration;

        public AccountController(Saml2Configuration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<IActionResult> AssertionConsumer()
        {
            var binding = new Saml2PostBinding();
            var response = new FixedSaml2AuthnResponse(_configuration);
            binding.Unbind(Request.ToGenericHttpRequest(), response);
            await response.CreateSession(HttpContext, ClaimsTransform: principal => ClaimsPrincipalHelper.Transform(principal));
            var returnUrl = binding.GetRelayStateQuery()[ReturnUrlRelayStateKey];
            return Redirect(returnUrl);
        }

        public IActionResult LogIn(string returnUrl)
        {
            var binding = new Saml2RedirectBinding();

            var relayState = new Dictionary<string, string>
            {
                { ReturnUrlRelayStateKey, returnUrl ?? Url.Action("Index", "Home") }   
            };

            binding.SetRelayStateQuery(relayState);
            var request = new Saml2AuthnRequest(_configuration);

            return binding.Bind(request)
                .ToActionResult();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                var binding = new Saml2PostBinding();
                var request = new Saml2LogoutRequest(_configuration);
                await request.DeleteSession(HttpContext);

                return binding.Bind(request)
                    .ToActionResult();
            }

            return Redirect(Url.Action("Index", "Home"));
        }

        public IActionResult LoggedOut()
        {
            var binding = new Saml2PostBinding();
            var response = new Saml2LogoutResponse(_configuration);
            binding.Unbind(Request.ToGenericHttpRequest(), response);
            return Redirect(Url.Action("Index", "Home"));
        }

        public async Task<IActionResult> SingleLogout()
        {
            var requestBinding = new Saml2PostBinding();
            var request = new Saml2LogoutRequest(_configuration);
            Saml2StatusCodes status;

            try
            {
                requestBinding.Unbind(Request.ToGenericHttpRequest(), request);
                await request.DeleteSession(HttpContext);
                status = Saml2StatusCodes.Success;
            }
            catch (Exception)
            {
                status = Saml2StatusCodes.RequestDenied;
            }

            var responseBinding = new Saml2PostBinding();
            responseBinding.RelayState = requestBinding.RelayState;

            var response = new Saml2LogoutResponse(_configuration)
            {
                InResponseToAsString = request.IdAsString,
                Status = status
            };

            return responseBinding.Bind(response)
                .ToActionResult();
        }
    }
}
