using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore.Configuration;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WingTipCommon.AspNetCore.Http;

namespace WingTipBillingWebApplication
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

            var loggerConfigurationSection = Configuration.GetSection("Logger");
            loggerFactory.AddConsole(loggerConfigurationSection);
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

            // Configure the JSON Web Token (JWT) bearer authentication for the Billing API.
            var jwtBearerAuthenticationConfigurationSection = Configuration.GetSection("JwtBearerAuthentication");
            ConfigureJwtBearerAuthentication(applicationBuilder, jwtBearerAuthenticationConfigurationSection);

            // Configure the SAML 2.0 authentication.
            applicationBuilder.UseSaml2();

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

            var saml2ConfigurationSection = Configuration.GetSection("Saml2");
            ConfigureSaml2(services, saml2ConfigurationSection);

            services.AddAuthorization(authorizationOptions =>
            {
                // Create an authorization policy for the Billing API that requires the "Billing.Read" scope to be contained in the
                // bearer JWT.
                authorizationOptions.AddPolicy(
                    Constants.AuthorizationPolicies.ReadBilling,
                    policy => policy.RequireAssertion(context =>
                    {
                        var scopeClaim = context.User.FindFirst("http://schemas.microsoft.com/identity/claims/scope");

                        if (scopeClaim == null)
                        {
                            return false;
                        }

                        return scopeClaim.Value.Split(' ').Contains("Billing.Read");
                    }));
            });

            services.AddMvc();
        }

        private static void ConfigureJwtBearerAuthentication(IApplicationBuilder applicationBuilder, IConfigurationSection jwtBearerAuthenticationConfigurationSection)
        {
            var jwtBearerAuthenticationOptions = new JwtBearerOptions
            {
                Audience = jwtBearerAuthenticationConfigurationSection["Audience"],
                MetadataAddress = jwtBearerAuthenticationConfigurationSection["MetadataAddress"]
            };

            applicationBuilder.UseJwtBearerAuthentication(jwtBearerAuthenticationOptions);
        }

        private static void ConfigureSaml2(IServiceCollection services, IConfigurationSection saml2ConfigurationSection)
        {
            var issuer = new Uri(saml2ConfigurationSection["Issuer"]);

            var saml2Configuration = new Saml2Configuration
            {
                Issuer = issuer,
                CertificateValidationMode = X509CertificateValidationMode.None, // This is not recommended for a production application
                SignatureAlgorithm = "http://www.w3.org/2000/09/xmldsig#rsa-sha1",
                RevocationMode = X509RevocationMode.NoCheck
            };

            saml2Configuration.AllowedAudienceUris.Add(issuer);

            // Read the identity provider SSO description from the B2C tenant.
            // The service provider SSO description is read by the target policy as follows:
            //     <Metadata>
            //         <Item Key="PartnerEntity">https://wingtipbillingb2c.azurewebsites.net/Metadata</Item>
            //         ...
            //     </Metadata>
            var entityDescriptor = new EntityDescriptor();
            entityDescriptor.ReadIdPSsoDescriptorFromUrl(new Uri(saml2ConfigurationSection["IdPMetadataUrl"]));

            if (entityDescriptor.IdPSsoDescriptor != null)
            {
                saml2Configuration.SingleSignOnDestination = entityDescriptor.IdPSsoDescriptor.SingleSignOnServices.First().Location;
                saml2Configuration.SingleLogoutDestination = entityDescriptor.IdPSsoDescriptor.SingleLogoutServices.First().Location;
                saml2Configuration.SignatureValidationCertificates.AddRange(entityDescriptor.IdPSsoDescriptor.SigningCertificates);
            }
            else
            {
                throw new InvalidOperationException("The federation metadata could not be loaded from metadata.");
            }

            services.AddSaml2(saml2Configuration);
        }
    }
}
