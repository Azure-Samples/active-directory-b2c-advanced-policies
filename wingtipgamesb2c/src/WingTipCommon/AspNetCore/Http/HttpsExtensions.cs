using System;
using Microsoft.AspNetCore.Builder;

namespace WingTipCommon.AspNetCore.Http
{
    public static class HttpsExtensions
    {
        public static IApplicationBuilder UseHttps(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
            {
                throw new ArgumentNullException(nameof(applicationBuilder));
            }

            return applicationBuilder.UseMiddleware<HttpsMiddleware>();
        }
    }
}
