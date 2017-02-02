using Owin;
using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using IdentityServer.Config;
using IdentityServer3.Core.Configuration;
using Microsoft.Owin.Security.OpenIdConnect;
using IdentityServer.Properties;
using IdentityServer3.Core.Services.Default;
using IdentityServer3.EntityFramework;
using CustomUserStore;
using IdentityServer3.Core.Services;

namespace IdentityServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        { 
            appBuilder.Map("/identity", identityServerAppBuilder =>
            {
                var identityServerServiceFactory = new IdentityServerServiceFactory();

                var entityFrameworkServiceOptions = new EntityFrameworkServiceOptions
                {
                    ConnectionString = ConfigurationManager.ConnectionStrings["CpimIdentityServerDbConnectionString"].ConnectionString
                };

                identityServerServiceFactory.RegisterClientStore(entityFrameworkServiceOptions);
                identityServerServiceFactory.UseInMemoryScopes(Scopes.Get());
                identityServerServiceFactory.UseInMemoryUsers(Users.Get());

                // Add custom user service
                var userService = new UserService();
                identityServerServiceFactory.UserService = new Registration<IUserService>(resolver => userService);

                var defaultViewServiceOptions = new DefaultViewServiceOptions
                {
                    CacheViews = false
                };

                defaultViewServiceOptions.Stylesheets.Add("/Styles/site.css");
                identityServerServiceFactory.ConfigureDefaultViewService(defaultViewServiceOptions);

                var options = new IdentityServerOptions
                {
                    LoggingOptions = new LoggingOptions()
                    {
                         WebApiDiagnosticsIsVerbose = true
                    }, 
                    AuthenticationOptions = new AuthenticationOptions
                    {
                        IdentityProviders = ConfigureIdentityProviders
                    },
                    Factory = identityServerServiceFactory,
                    IssuerUri = Settings.Default.IdentityServerRedirectUri,
                    PublicOrigin = Settings.Default.Origin,
                    RequireSsl = false,
                    SigningCertificate = LoadCertificate(),
                    SiteName = Settings.Default.SiteName
                };

                identityServerAppBuilder.UseIdentityServer(options);
                ConfigureMvc();
            });                      
        }

        private static void ConfigureIdentityProviders(IAppBuilder appBuilder, string signInAsType)
        {
            var oidc = new OpenIdConnectAuthenticationOptions
            {
                Notifications = new OpenIdConnectAuthenticationNotifications()
                {
                    RedirectToIdentityProvider = context =>
                    {
                        // The open id class can't deal with authorization uri which already contain '?'
                        // We need this work around to cover it in the request
                        var parts = context.ProtocolMessage.IssuerAddress.Split(new[] { '?' });
                        context.ProtocolMessage.IssuerAddress = parts[0];
                        if (parts.Length > 1)
                        {
                            context.ProtocolMessage.Parameters.Add("p", Settings.Default.B2CPolicy);
                        }

                        return Task.FromResult(0);
                    },
                    
                    SecurityTokenValidated = context =>
                    {
                        return Task.FromResult(0);
                    },
                },
                RedirectUri = Settings.Default.IdentityServerRedirectUri,
                Scope = "openid",
                ResponseType= "id_token",
                Caption = Settings.Default.B2CAuthProviderName,
                SignInAsAuthenticationType = signInAsType,
                ClientId = Settings.Default.B2CClientId,
                MetadataAddress = string.Format(Settings.Default.B2CMetadata, Settings.Default.B2CTenant, Settings.Default.B2CPolicy),             
                ClientSecret = @Settings.Default.B2CClientSecret
            };
            appBuilder.UseOpenIdConnectAuthentication(oidc);
        }

        private static void ConfigureMvc()
        {
            ConfigureMvcAreas();
            ConfigureMvcBundles(BundleTable.Bundles);
            ConfigureMvcRoutes(RouteTable.Routes);
        }

        private static void ConfigureMvcAreas()
        {
            AreaRegistration.RegisterAllAreas();
        }

        private static void ConfigureMvcBundles(BundleCollection bundles)
        {
            var styleBundle = new StyleBundle("~/bundles/css").Include("~/Styles/site.css");
            bundles.Add(styleBundle);
        }

        private static void ConfigureMvcRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Default", "{controller}/{action}/{id}", new
            {
                controller = "Home",
                action = "Index",
                id = UrlParameter.Optional
            });
        }

        private static X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2($@"{AppDomain.CurrentDomain.BaseDirectory}\certificates\idsrv3test.pfx", "idsrv3test");
        }
    }
}
