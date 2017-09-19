using System;
using Microsoft.AspNetCore.Builder;

namespace WingTipIdentityWebApplication.Authentication
{
    public static class BasicAuthenticationExtensions
    {
        public static IApplicationBuilder UseBasicAuthentication(this IApplicationBuilder applicationBuilder, string clientId, string clientSecret)
        {
            if (applicationBuilder == null)
            {
                throw new ArgumentNullException(nameof(applicationBuilder));
            }

            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }

            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentNullException(nameof(clientSecret));
            }

            return applicationBuilder.UseMiddleware<BasicAuthenticationMiddleware>(clientId, clientSecret);
        }
    }
}
