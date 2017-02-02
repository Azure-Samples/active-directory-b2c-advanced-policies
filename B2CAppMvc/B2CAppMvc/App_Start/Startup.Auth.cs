using B2CAppMvc.Properties;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace B2CAppMvc
{
    public partial class Startup
    {
        public void ConfigureAuthentication(IAppBuilder appBuilder)
        {
            appBuilder.SetDefaultSignInAsAuthenticationType(DefaultAuthenticationTypes.ApplicationCookie);

            appBuilder.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });

            // Add policies for the Azure Active Directory B2C tenant.
            for (var index = Settings.Default.B2CPolicy.Count - 1; index >= 0; index--)
            {
                var oidc = new B2COpenidConnect(
                    Settings.Default.Caption[index], 
                    Settings.Default.B2CTenant, 
                    Settings.Default.B2CPolicy[index], 
                    Settings.Default.B2CClientId, 
                    Settings.Default.B2CClientSecret,
                    Settings.Default.B2CMetadata,
                    Settings.Default.AppRedirectUri,
                    Settings.Default.AddClientClaimsToPolicy);
                appBuilder.UseOpenIdConnectAuthentication(oidc.Options);
            }
        }
    }
}
