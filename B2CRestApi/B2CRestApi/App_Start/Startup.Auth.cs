using B2CRestApi.Basic_Authentication;
using B2CRestApi.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace B2CRestApi
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Remove the default cookie based authentication and replace it by our custom middleware
            // We don't need user authentication but just client authentication
            app.Use<ClientAuthMiddleware>();

        }
    }
}
