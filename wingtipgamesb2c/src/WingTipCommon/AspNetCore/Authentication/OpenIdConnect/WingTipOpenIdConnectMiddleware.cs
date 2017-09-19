using System;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WingTipCommon.AspNetCore.Authentication.OpenIdConnect
{
    public class WingTipOpenIdConnectMiddleware : OpenIdConnectMiddleware
    {
        public WingTipOpenIdConnectMiddleware(
            RequestDelegate next,
            IDataProtectionProvider dataProtectionProvider,
            ILoggerFactory loggerFactory,
            UrlEncoder urlEncoder,
            IServiceProvider serviceProvider,
            IOptions<SharedAuthenticationOptions> sharedOptions,
            IOptions<OpenIdConnectOptions> options,
            HtmlEncoder htmlEncoder)
            : base(next, dataProtectionProvider, loggerFactory, urlEncoder, serviceProvider, sharedOptions, options, htmlEncoder)
        {
        }

        protected override AuthenticationHandler<OpenIdConnectOptions> CreateHandler()
        {
            return new WingTipOpenIdConnectHandler(Backchannel, HtmlEncoder);
        }
    }
}
