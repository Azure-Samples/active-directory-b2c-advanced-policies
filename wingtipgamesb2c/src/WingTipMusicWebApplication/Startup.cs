using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using WingTipCommon.AspNetCore.Http;
using WingTipMusicWebApplication.Filters;
using WingTipMusicWebApplication.Repositories;
using WingTipMusicWebApplication.Services;

namespace WingTipMusicWebApplication
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

            // Configure the JSON Web Token (JWT) bearer authentication for the Music API.
            var jwtBearerAuthenticationConfigurationSection = Configuration.GetSection("JwtBearerAuthentication");
            ConfigureJwtBearerAuthentication(applicationBuilder, jwtBearerAuthenticationConfigurationSection);

            // Configure the cookie authentication.
            ConfigureCookieAuthentication(applicationBuilder);

            // Configure the OpenID Connect authentication middleware for the B2C policies...
            var openIdConnectAuthenticationConfigurationSection = Configuration.GetSection("OpenIdConnectAuthentication");

            // Configure the OpenID Connect authentication middleware for the "b2c_1a_password_reset" policy that is used for resetting
            // the password for a local account.
            ConfigureOpenIdConnectAuthentication(
                applicationBuilder,
                openIdConnectAuthenticationConfigurationSection,
                Constants.PolicyIds.PasswordReset,
                null);

            // Configure the OpenID Connect authentication middleware for the "b2c_1a_profile_update_music" policy that is used for
            // updating the player profile for a user.
            ConfigureOpenIdConnectAuthentication(
                applicationBuilder,
                openIdConnectAuthenticationConfigurationSection,
                Constants.PolicyIds.ProfileUpdate,
                null);

            // Configure the OpenID Connect authentication middleware for the "b2c_1a_sign_up_sign_in_music" policy that is used for
            // logging in or registering a user.
            ConfigureOpenIdConnectAuthentication(
                applicationBuilder,
                openIdConnectAuthenticationConfigurationSection,
                Constants.PolicyIds.SignUpOrSignIn,
                new List<Claim> // Initialize a list of claims to be sent to this policy
                {
                    new Claim("brand", "WingTip Music")
                });

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

            services.AddAuthorization(authorizationOptions =>
            {
                // Create an authorization policy for the Games API that requires the "Music.Read" scope to be contained in the bearer
                // JWT.
                authorizationOptions.AddPolicy(
                    Constants.AuthorizationPolicies.ReadMusic,
                    policy => policy.RequireAssertion(context =>
                    {
                        var scopeClaim = context.User.FindFirst("http://schemas.microsoft.com/identity/claims/scope");

                        if (scopeClaim == null)
                        {
                            return false;
                        }

                        return scopeClaim.Value.Split(' ').Contains("Music.Read");
                    }));
            });

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(CultureFilterAttribute));
                options.Filters.Add(typeof(LocationFilterAttribute));
            });

            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            services.AddTransient<IAlbumRepository, AlbumRepository>();
            var geolocationService = new FreeGeoIpGeolocationService();
            services.AddSingleton<IGeolocationService>(geolocationService);
        }

        private static void ConfigureCookieAuthentication(IApplicationBuilder applicationBuilder)
        {
            var cookieAuthenticationOptions = new CookieAuthenticationOptions
            {
                AuthenticationScheme = Constants.AuthenticationSchemes.ApplicationCookie,
                CookieName = $".WingTipGamesWebApplication.{Constants.AuthenticationSchemes.ApplicationCookie}"
            };

            applicationBuilder.UseCookieAuthentication(cookieAuthenticationOptions);
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

        private static void ConfigureOpenIdConnectAuthentication(
            IApplicationBuilder applicationBuilder,
            IConfigurationSection openIdConnectAuthenticationConfigurationSection,
            string policyId,
            ICollection<Claim> staticPolicyClaims)
        {
            var tenantId = openIdConnectAuthenticationConfigurationSection["TenantId"];

            var openIdConnectAuthenticationOptions = new OpenIdConnectOptions
            {
                AuthenticationScheme = policyId,
                CallbackPath = $"/{policyId}-callback", // This sign-in callback path must be registered as a redirect URI for the B2C application
                ClientId = openIdConnectAuthenticationConfigurationSection["ClientId"],
                ClientSecret = openIdConnectAuthenticationConfigurationSection["ClientSecret"],
                Events = new OpenIdConnectEvents
                {
                    OnMessageReceived = context =>
                    {
                        // If an error has been raised, then remember the return URL for use by the OnRemoteFailure event.
                        if (!string.IsNullOrEmpty(context.ProtocolMessage.Error) &&
                            context.ProtocolMessage.Error.Equals("access_denied") &&
                            context.ProtocolMessage.ErrorDescription.StartsWith("AADB2C90118"))
                        {
                            context.HttpContext.Items["redirect_uri"] = context.Properties.RedirectUri;
                        }

                        return Task.FromResult(0);
                    },
                    OnRedirectToIdentityProvider = async context =>
                    {
                        context.ProtocolMessage.Scope = openIdConnectAuthenticationConfigurationSection["Scope"];

                        // If an Account controller action has set a UI locale (e.g. to "fr"), then set the UI locales parameter for
                        // the authentication request to this UI locale.
                        if (context.Properties.Items.ContainsKey(Constants.AuthenticationPropertiesKeys.UILocales))
                        {
                            context.ProtocolMessage.UiLocales = context.Properties.Items[Constants.AuthenticationPropertiesKeys.UILocales];
                        }

                        // If an Account controller action has set a domain hint (e.g. to "facebook.com"), then set the domain hint
                        // parameter for the authentication request to this domain hint.
                        if (context.Properties.Items.ContainsKey(Constants.AuthenticationPropertiesKeys.DomainHint))
                        {
                            context.ProtocolMessage.DomainHint = context.Properties.Items[Constants.AuthenticationPropertiesKeys.DomainHint];
                        }

                        // If a list of claims are to be sent to the target policy, then...
                        if (staticPolicyClaims != null)
                        {
                            var configuration = await context.Options.ConfigurationManager.GetConfigurationAsync(CancellationToken.None);

                            // Initialize another list of claims to be sent.
                            var instancePolicyClaims = new List<Claim>(staticPolicyClaims);

                            // Create a self-issued JSON Web Token (JWT) containing the list of claims and signed by the client secret.
                            var selfIssuedToken = CreateSelfIssuedToken(
                                configuration.Issuer,
                                context.ProtocolMessage.RedirectUri,
                                context.Options.ClientSecret,
                                instancePolicyClaims);

                            // Set the client assertion parameter for the authentication request to this JWT.
                            // The list of claims is received by the target policy as follows:
                            //     <InputTokenFormat>JWT</InputTokenFormat>
                            //     <CryptographicKeys>
                            //         <Key Id="client_secret" StorageReferenceId="WingTipMusicClientSecret" />
                            //     </CryptographicKeys>
                            //     <InputClaims>
                            //         <InputClaim ClaimTypeReferenceId="extension_Brand" />
                            //       </InputClaims>
                            context.ProtocolMessage.Parameters.Add("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer");
                            context.ProtocolMessage.Parameters.Add("client_assertion", selfIssuedToken);
                        }
                    },
                    OnRemoteFailure = context =>
                    {
                        // Handle the error that is raised when a user has requested to recover a password.
                        if (!string.IsNullOrEmpty(context.Failure.Message) &&
                            context.Failure.Message.Contains("access_denied") &&
                            context.Failure.Message.Contains("AADB2C90118"))
                        {
                            context.Response.Redirect($"/Account/RecoverPassword?ReturnUrl={context.HttpContext.Items["redirect_uri"]}");
                            context.HandleResponse();
                        }

                        // Handle any other error that is raised.
                        if (!string.IsNullOrEmpty(context.Failure.Message) &&
                            context.Failure.Message.Contains("access_denied") &&
                            context.Failure.Message.Contains("AADB2C90091"))
                        {
                            context.Response.Redirect("/");
                            context.HandleResponse();
                        }

                        return Task.FromResult(0);
                    }
                },
                MetadataAddress = $"https://login.microsoftonline.com/{tenantId}/v2.0/.well-known/openid-configuration?p={policyId}",
                PostLogoutRedirectUri = "/",
                SignInScheme = Constants.AuthenticationSchemes.ApplicationCookie,
                SignedOutCallbackPath = $"/{policyId}-signout-callback", // This sign-out callback path does not have to be registered as a redirect URI for the B2C application
                SignOutScheme = Constants.AuthenticationSchemes.ApplicationCookie
            };

            applicationBuilder.UseOpenIdConnectAuthentication(openIdConnectAuthenticationOptions);
        }

        private static string CreateSelfIssuedToken(
            string issuer,
            string audience,
            string signingSecret,
            ICollection<Claim> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var nowUtc = DateTime.UtcNow;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingSecret));
            var signingCredentials = new SigningCredentials(key, "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = audience,
                Expires = nowUtc.AddMinutes(5),
                IssuedAt = nowUtc,
                Issuer = issuer,
                NotBefore = nowUtc,
                SigningCredentials = signingCredentials,
                Subject = new ClaimsIdentity(claims)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
