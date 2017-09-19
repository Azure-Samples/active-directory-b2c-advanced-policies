using System;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace WingTipIdentityWebApplication.Authentication
{
    public class BasicAuthenticationMiddleware
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly RequestDelegate _next;

        public BasicAuthenticationMiddleware(RequestDelegate next, string clientId, string clientSecret)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }

            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentNullException(nameof(clientSecret));
            }

            _next = next;
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            string clientId;
            string clientSecret;

            if (TryGetBasicCredentials(context.Request, out clientId, out clientSecret) && TryValidateBasicCredentials(clientId, clientSecret))
            {
                var identity = new GenericIdentity(clientId, "Basic");
                context.User = new GenericPrincipal(identity, new[] { "Client" });

                if (_next != null)
                {
                    await _next.Invoke(context);
                }
            }
            else
            {
                context.Response.StatusCode = 401;
            }
        }

        private static bool TryGetBasicCredentials(HttpRequest request, out string clientId, out string clientSecret)
        {
            var authorizationHeaderValues = request.Headers["Authorization"];
            var authorizationHeaderValue = authorizationHeaderValues.FirstOrDefault(x => x.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(authorizationHeaderValue))
            {
                try
                {
                    var authorizationHeaderSchemeAndParameter = authorizationHeaderValue.Split(' ');

                    if (authorizationHeaderSchemeAndParameter.Length == 2)
                    {
                        var authorizationHeaderParameter = authorizationHeaderSchemeAndParameter[1];

                        if (!string.IsNullOrEmpty(authorizationHeaderParameter))
                        {
                            var clientIdAndSecret = Encoding.UTF8.GetString(Convert.FromBase64String(authorizationHeaderParameter)).Split(':');

                            if (clientIdAndSecret.Length == 2)
                            {
                                clientId = clientIdAndSecret[0];
                                clientSecret = clientIdAndSecret[1];
                                return true;
                            }
                        }
                    }
                }
                catch (FormatException)
                {
                }
                catch (ArgumentException)
                {
                }
            }

            clientId = string.Empty;
            clientSecret = string.Empty;
            return false;
        }

        private bool TryValidateBasicCredentials(string clientId, string clientSecret)
        {
            return clientId == _clientId && clientSecret == _clientSecret;
        }
    }
}
