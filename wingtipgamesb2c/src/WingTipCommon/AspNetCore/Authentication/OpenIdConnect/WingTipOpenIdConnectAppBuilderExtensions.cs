using System;
using Microsoft.Extensions.Options;
using WingTipCommon.AspNetCore.Authentication.OpenIdConnect;

namespace Microsoft.AspNetCore.Builder
{
    public static class WingTipOpenIdConnectAppBuilderExtensions
    {
        public static IApplicationBuilder UseWingTipOpenIdConnectAuthentication(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
            {
                throw new ArgumentNullException(nameof(applicationBuilder));
            }

            return applicationBuilder.UseMiddleware<WingTipOpenIdConnectMiddleware>();
        }

        public static IApplicationBuilder UseWingTipOpenIdConnectAuthentication(
            this IApplicationBuilder applicationBuilder,
            OpenIdConnectOptions options)
        {
            if (applicationBuilder == null)
            {
                throw new ArgumentNullException(nameof(applicationBuilder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return applicationBuilder.UseMiddleware<WingTipOpenIdConnectMiddleware>(Options.Create(options));
        }
    }
}
