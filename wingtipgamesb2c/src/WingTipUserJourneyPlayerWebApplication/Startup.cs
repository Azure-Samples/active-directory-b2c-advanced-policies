using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WingTipCommon.AspNetCore.Http;

namespace WingTipUserJourneyPlayerWebApplication
{
    public class Startup
    {
        public Startup(IHostingEnvironment hostingEnvironment)
        {
            if (hostingEnvironment == null)
            {
                throw new ArgumentNullException(nameof(hostingEnvironment));
            }

            Configuration = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void Configure(IApplicationBuilder applicationBuilder, IHostingEnvironment hostingEnvironment, ILoggerFactory loggerFactory)
        {
            if (applicationBuilder == null)
            {
                throw new ArgumentNullException(nameof(applicationBuilder));
            }

            if (hostingEnvironment == null)
            {
                throw new ArgumentNullException(nameof(hostingEnvironment));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            loggerFactory.AddConsole(Configuration.GetSection("Logger"));
            loggerFactory.AddDebug();
            applicationBuilder.UseHttps();

            if (hostingEnvironment.IsDevelopment())
            {
                applicationBuilder.UseDeveloperExceptionPage();
                applicationBuilder.UseBrowserLink();
            }
            else
            {
                applicationBuilder.UseExceptionHandler("/Home/Error");
            }

            ConfigureCookieAuthentication(applicationBuilder);
            var openIdConnectAuthenticationConfigurationSection = Configuration.GetSection("OpenIdConnectAuthentication");
            ConfigureOpenIdConnectAuthentication(applicationBuilder, openIdConnectAuthenticationConfigurationSection);
            applicationBuilder.UseStaticFiles();

            applicationBuilder.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Trace}/{action=Index}/{id?}");
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var azureApplicationInsightsAuthentication = Configuration.GetSection("AzureApplicationInsightsAuthentication");

            var azureApplicationInsightsCredential = new AzureApplicationInsightsCredential
            {
                ApplicationId = azureApplicationInsightsAuthentication["ApplicationId"],
                ApiKey = azureApplicationInsightsAuthentication["ApiKey"]
            };

            services.AddSingleton(azureApplicationInsightsCredential);

            services.AddMvc(options =>
            {
                var authorizationPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                var authorizeFilter = new AuthorizeFilter(authorizationPolicy);
                options.Filters.Add(authorizeFilter);
            });
        }

        private static void ConfigureCookieAuthentication(IApplicationBuilder applicationBuilder)
        {
            var cookieAuthenticationOptions = new CookieAuthenticationOptions
            {
                AuthenticationScheme = Constants.AuthenticationSchemes.ApplicationCookie,
                CookieName = $".WingTipUserJourneyPlayerWebApplication.{Constants.AuthenticationSchemes.ApplicationCookie}"
            };

            applicationBuilder.UseCookieAuthentication(cookieAuthenticationOptions);
        }

        private static void ConfigureOpenIdConnectAuthentication(IApplicationBuilder applicationBuilder, IConfigurationSection openIdConnectAuthenticationConfigurationSection)
        {
            var tenantId = openIdConnectAuthenticationConfigurationSection["TenantId"];

            var openIdConnectAuthenticationOptions = new OpenIdConnectOptions
            {
                ClientId = openIdConnectAuthenticationConfigurationSection["ClientId"],
                ClientSecret = openIdConnectAuthenticationConfigurationSection["ClientSecret"],
                MetadataAddress = $"https://login.microsoftonline.com/{tenantId}/.well-known/openid-configuration",
                ResponseType = "code id_token",
                SignInScheme = Constants.AuthenticationSchemes.ApplicationCookie,
                SignOutScheme = Constants.AuthenticationSchemes.ApplicationCookie
            };

            applicationBuilder.UseOpenIdConnectAuthentication(openIdConnectAuthenticationOptions);
        }
    }
}
