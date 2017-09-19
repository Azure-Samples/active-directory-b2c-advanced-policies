using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WingTipCommon.AspNetCore.Http
{
    public class HttpsMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var request = context.Request;

            if (!request.IsHttps)
            {
                var location = "https://" + request.Host + request.Path + request.QueryString;
                context.Response.Redirect(location);
            }
            else
            {
                await _next(context);
            }
        }
    }
}
