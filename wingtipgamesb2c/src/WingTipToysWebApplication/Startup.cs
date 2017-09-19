using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using WingTipCommon.AspNetCore.Http;
using WingTipCommon.Generators;
using WingTipCommon.Identity;
using WingTipCommon.Services;
using WingTipToysWebApplication.Configuration;

namespace WingTipToysWebApplication
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
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", true)
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
                applicationBuilder.UseBrowserLink();
                applicationBuilder.UseDeveloperExceptionPage();
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
                routes.MapRoute("Default", "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.Configure<UserControllerOptions>(o =>
            {
                var activationConfigurationSection = Configuration.GetSection("Activation");
                o.ActivationAction = activationConfigurationSection["Action"];
                o.ActivationController = activationConfigurationSection["Controller"];
                o.ActivationHost = activationConfigurationSection["Host"];
                o.ActivationKey = activationConfigurationSection["Key"];
            });

            services.AddSingleton<IAuthenticationService>(serviceProvider =>
            {
                var openIdConnectAuthenticationConfigurationSection = Configuration.GetSection("OpenIdConnectAuthentication");
                return new AuthenticationService(openIdConnectAuthenticationConfigurationSection["TenantId"]);
            });

            services.AddTransient<IGraphService>(serviceProvider =>
            {
                var openIdConnectAuthenticationConfigurationSection = Configuration.GetSection("OpenIdConnectAuthentication");

                return new GraphService(
                    serviceProvider.GetRequiredService<IAuthenticationService>(),
                    openIdConnectAuthenticationConfigurationSection["TenantId"],
                    openIdConnectAuthenticationConfigurationSection["ClientId"],
                    openIdConnectAuthenticationConfigurationSection["ClientSecret"],
                    new Uri(openIdConnectAuthenticationConfigurationSection["RedirectUri"]));
            });

            services.AddSingleton<IPasswordGenerator>(serviceProvider => new PasswordGenerator(20, PasswordCharacters.Alphanumeric));
            var smtpServiceConfigurationSection = Configuration.GetSection("SendGridSmtpService");

            var smtpService = new SendGridSmtpService(
                true,
                smtpServiceConfigurationSection["Host"],
                int.Parse(smtpServiceConfigurationSection["Port"]),
                smtpServiceConfigurationSection["User"],
                smtpServiceConfigurationSection["Pass"]);

            services.AddSingleton<ISmtpService>(smtpService);
            services.AddDbContext<WingTipDbContext>(optionsBuilder => optionsBuilder.UseSqlServer(Configuration.GetConnectionString("WingTipB2CDbConnectionString")));

            services.AddIdentity<WingTipUser, IdentityRole>(identityOptions =>
            {
                identityOptions.Password.RequireDigit = false;
                identityOptions.Password.RequireLowercase = false;
                identityOptions.Password.RequireNonAlphanumeric = false;
                identityOptions.Password.RequireUppercase = false;
                identityOptions.Password.RequiredLength = 8;
            }).AddEntityFrameworkStores<WingTipDbContext>();

            services.AddScoped<WingTipUserManager>();
            services.AddMvc();
        }

        private static void ConfigureCookieAuthentication(IApplicationBuilder applicationBuilder)
        {
            var cookieAuthenticationOptions = new CookieAuthenticationOptions
            {
                AuthenticationScheme = Constants.AuthenticationSchemes.ApplicationCookie,
                CookieName = $".WingTipToysWebApplication.{Constants.AuthenticationSchemes.ApplicationCookie}"
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
                Events = new OpenIdConnectEvents
                {
                    OnAuthorizationCodeReceived = async context =>
                    {
                        var authenticationService = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationService>();
                        var clientCredential = new ClientCredential(context.Options.ClientId, context.Options.ClientSecret);

                        var authenticationResult = await authenticationService.AcquireTokenByAuthorizationCodeAsync(
                            context.ProtocolMessage.Code,
                            new Uri(context.Properties.Items[OpenIdConnectDefaults.RedirectUriForCodePropertiesKey]),
                            clientCredential,
                            openIdConnectAuthenticationConfigurationSection["Resource"]);

                        context.HandleCodeRedemption(authenticationResult.AccessToken, authenticationResult.IdToken);
                    }
                },
                MetadataAddress = $"https://login.microsoftonline.com/{tenantId}/.well-known/openid-configuration",
                ResponseType = "code id_token",
                SignInScheme = Constants.AuthenticationSchemes.ApplicationCookie,
                SignOutScheme = Constants.AuthenticationSchemes.ApplicationCookie
            };

            applicationBuilder.UseOpenIdConnectAuthentication(openIdConnectAuthenticationOptions);
        }
    }
}
