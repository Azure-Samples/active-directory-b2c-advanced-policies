using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WingTipCommon.AspNetCore.Http;
using WingTipCommon.Generators;
using WingTipCommon.Identity;
using WingTipCommon.Services;
using WingTipIdentityWebApplication.Authentication;

namespace WingTipIdentityWebApplication
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

            // Configure the basic authentication for the Identity API.
            var basicAuthenticationConfigurationSection = Configuration.GetSection("BasicAuthentication");
            applicationBuilder.UseBasicAuthentication(basicAuthenticationConfigurationSection["ClientId"], basicAuthenticationConfigurationSection["ClientSecret"]);

            applicationBuilder.UseMvc();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IAuthenticationService>(serviceProvider =>
            {
                var activeDirectoryAuthenticationConfigurationSection = Configuration.GetSection("ActiveDirectoryAuthentication");
                return new AuthenticationService(activeDirectoryAuthenticationConfigurationSection["TenantId"]);
            });

            services.AddTransient<IGraphService>(serviceProvider =>
            {
                var authenticationService = serviceProvider.GetRequiredService<IAuthenticationService>();
                var activeDirectoryAuthenticationConfigurationSection = Configuration.GetSection("ActiveDirectoryAuthentication");

                return new GraphService(
                    authenticationService,
                    activeDirectoryAuthenticationConfigurationSection["TenantId"],
                    activeDirectoryAuthenticationConfigurationSection["ClientId"],
                    activeDirectoryAuthenticationConfigurationSection["ClientSecret"]);
            });

            services.AddSingleton<IPasswordGenerator>(serviceProvider => new PasswordGenerator(20, PasswordCharacters.Alphanumeric));
            services.AddDbContext<WingTipDbContext>(optionsBuilder => optionsBuilder.UseSqlServer(Configuration.GetConnectionString("WingTipB2CDbConnectionString")));

            // Soften the password policy so that passwords can be set from a B2C policy.
            services.AddIdentity<WingTipUser, IdentityRole>(identityOptions =>
            {
                identityOptions.Password.RequireDigit = false;
                identityOptions.Password.RequireLowercase = false;
                identityOptions.Password.RequireNonAlphanumeric = false;
                identityOptions.Password.RequireUppercase = false;
                identityOptions.Password.RequiredLength = 1;
            }).AddEntityFrameworkStores<WingTipDbContext>();

            services.AddScoped<WingTipUserManager>();
            services.AddMvc();
        }
    }
}
