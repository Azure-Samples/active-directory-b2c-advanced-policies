using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace B2CRestApi.Basic_Authentication
{
    /// <summary>
    /// Class to create a custom owin middleware to check for client authentication
    /// </summary>
    public class ClientAuthMiddleware
    {
        /// <summary>
        /// Gets or sets the next owin middleware
        /// </summary>
        private Func<IDictionary<string, object>, Task> Next { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientAuthMiddleware"/> class.
        /// </summary>
        /// <param name="next"></param>
        public ClientAuthMiddleware(Func<IDictionary<string, object>, Task> next)
        {
            this.Next = next;
        }

        /// <summary>
        /// Invoke client authentication middleware during each request.
        /// </summary>
        /// <param name="environment">Owin environment</param>
        /// <returns></returns>
        public Task Invoke(IDictionary<string, object> environment)
        {
            // Get wrapper class for the environment
            var context = new OwinContext(environment);

            // Check whether the authorization header is available. This contains the credentials.
            var authzValue = context.Request.Headers.Get("Authorization");
            if (string.IsNullOrEmpty(authzValue) || !authzValue.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                // Process next middleware
                return Next(environment);
            }

            // Get credentials
            var creds = authzValue.Substring("Basic ".Length).Trim();
            string clientId;
            string clientSecret;

            if (RetrieveCreds(creds, out clientId, out clientSecret))
            {
                // Set transaction authenticated as client
                context.Request.User = new GenericPrincipal(new GenericIdentity(clientId, "client"), new string[] { "client" });
            }

            return Next(environment);
        }

        /// <summary>
        /// Retrieve credentials from header
        /// </summary>
        /// <param name="credentials">Authorization header</param>
        /// <param name="clientId">Client identifier</param>
        /// <param name="clientSecret">Client secret</param>
        /// <returns>True if valid credentials were presented</returns>
        private bool RetrieveCreds(string credentials, out string clientId, out string clientSecret)
        {
            string pair;
            clientId = clientSecret = string.Empty;

            try
            {
                pair = Encoding.UTF8.GetString(Convert.FromBase64String(credentials));
            }
            catch (FormatException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            var ix = pair.IndexOf(':');
            if (ix == -1)
            {
                return false;
            }

            clientId = pair.Substring(0, ix);
            clientSecret = pair.Substring(ix + 1);

            // Return whether credentials are valid
            return (string.Compare(clientId, Properties.Settings.Default.ClientId) == 0 &&
                string.Compare(clientSecret, Properties.Settings.Default.ClientSecret) == 0);
        }
    }
}