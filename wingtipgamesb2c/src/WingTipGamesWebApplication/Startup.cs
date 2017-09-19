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
using WingTipCommon.Services;
using WingTipGamesWebApplication.Configuration;
using WingTipGamesWebApplication.Filters;
using WingTipGamesWebApplication.Repositories;
using WingTipGamesWebApplication.Services;

namespace WingTipGamesWebApplication
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

            // Configure the JSON Web Token (JWT) bearer authentication for the Games API.
            var jwtBearerAuthenticationConfigurationSection = Configuration.GetSection("JwtBearerAuthentication");
            ConfigureJwtBearerAuthentication(applicationBuilder, jwtBearerAuthenticationConfigurationSection);

            // Configure the cookie authentication.
            ConfigureCookieAuthentication(applicationBuilder);

            // Configure the OpenID Connect authentication middleware for the B2C policies...
            var openIdConnectAuthenticationConfigurationSection = Configuration.GetSection("OpenIdConnectAuthentication");

            // Configure the OpenID Connect authentication middleware for the "b2c_1a_activation" policy that is used for inviting a
            // user.
            ConfigureOpenIdConnectAuthenticationWithRedirect(
                applicationBuilder,
                Constants.PolicyIds.Activation,
                openIdConnectAuthenticationConfigurationSection,
                Constants.PolicyIds.Activation,
                new List<Claim> // Initialize a list of claims to be sent to this policy
                {
                    new Claim("brand", "WingTip Games")
                });

            // Configure the OpenID Connect authentication middleware for the "b2c_1a_invitation" policy that is used for inviting a
            // user via an email link.
            ConfigureOpenIdConnectAuthenticationWithLink(
                applicationBuilder,
                Constants.PolicyIds.InvitationLink,
                openIdConnectAuthenticationConfigurationSection,
                Constants.PolicyIds.Invitation,
                new List<Claim> // Initialize a list of claims to be sent to this policy
                {
                    new Claim("brand", "WingTip Games")
                });

            // Configure the OpenID Connect authentication middleware for the "b2c_1a_invitation" policy that is used for inviting a
            // user via a HTTP redirect.
            ConfigureOpenIdConnectAuthenticationWithRedirect(
                applicationBuilder,
                Constants.PolicyIds.InvitationRedirect,
                openIdConnectAuthenticationConfigurationSection,
                Constants.PolicyIds.Invitation,
                new List<Claim> // Initialize a list of claims to be sent to this policy
                {
                    new Claim("brand", "WingTip Games")
                });

            // Configure the OpenID Connect authentication middleware for the "b2c_1a_link" policy that is used for linking the local
            // account for a user to the social account for the user.
            ConfigureOpenIdConnectAuthenticationWithRedirect(
                applicationBuilder,
                Constants.PolicyIds.Link,
                openIdConnectAuthenticationConfigurationSection,
                Constants.PolicyIds.Link,
                null);

            // Configure the OpenID Connect authentication middleware for the "b2c_1a_password_reset" policy that is used for resetting
            // the password for a local account.
            ConfigureOpenIdConnectAuthenticationWithRedirect(
                applicationBuilder,
                Constants.PolicyIds.PasswordReset,
                openIdConnectAuthenticationConfigurationSection,
                Constants.PolicyIds.PasswordReset,
                null);

            // Configure the OpenID Connect authentication middleware for the "b2c_1a_profile_update_games" policy that is used for
            // updating the player profile for a user.
            ConfigureOpenIdConnectAuthenticationWithRedirect(
                applicationBuilder,
                Constants.PolicyIds.ProfileUpdate,
                openIdConnectAuthenticationConfigurationSection,
                Constants.PolicyIds.ProfileUpdate,
                null);

            // Configure the OpenID Connect authentication middleware for the "b2c_1a_sign_in_games_app_code" policy that is used
            // for logging in a user using an app code.
            ConfigureOpenIdConnectAuthenticationWithRedirect(
                applicationBuilder,
                Constants.PolicyIds.SignInUsingAppCode,
                openIdConnectAuthenticationConfigurationSection,
                Constants.PolicyIds.SignInUsingAppCode,
                null);

            // Configure the OpenID Connect authentication middleware for the "b2c_1a_sign_in_games_authy_code" policy that is used
            // for logging in a user using an app code.
            ConfigureOpenIdConnectAuthenticationWithRedirect(
                applicationBuilder,
                Constants.PolicyIds.SignInUsingAuthyCode,
                openIdConnectAuthenticationConfigurationSection,
                Constants.PolicyIds.SignInUsingAuthyCode,
                null);

            // Configure the OpenID Connect authentication middleware for the "b2c_1a_sign_in_games_email_code" policy that is used
            // for logging in a user using an email code.
            ConfigureOpenIdConnectAuthenticationWithRedirect(
                applicationBuilder,
                Constants.PolicyIds.SignInUsingEmailCode,
                openIdConnectAuthenticationConfigurationSection,
                Constants.PolicyIds.SignInUsingEmailCode,
                null);

            // Configure the OpenID Connect authentication middleware for the "b2c_1a_sign_in_games_phone_code" policy that is used
            // for logging in a user using a phone code.
            ConfigureOpenIdConnectAuthenticationWithRedirect(
                applicationBuilder,
                Constants.PolicyIds.SignInUsingPhoneCode,
                openIdConnectAuthenticationConfigurationSection,
                Constants.PolicyIds.SignInUsingPhoneCode,
                null);

            // Configure the OpenID Connect authentication middleware for the "b2c_1a_sign_up_sign_in_games" policy that is used for
            // logging in or registering a user.
            ConfigureOpenIdConnectAuthenticationWithRedirect(
                applicationBuilder,
                Constants.PolicyIds.SignUpOrSignIn,
                openIdConnectAuthenticationConfigurationSection,
                Constants.PolicyIds.SignUpOrSignIn,
                new List<Claim> // Initialize a list of claims to be sent to this policy
                {
                    new Claim("brand", "WingTip Games")
                });

            // Configure the OpenID Connect authentication middleware for the "b2c_1a_sign_up_games_app_code" policy that is used for
            // registering a user using an app code.
            ConfigureOpenIdConnectAuthenticationWithRedirect(
                applicationBuilder,
                Constants.PolicyIds.SignUpUsingAppCode,
                openIdConnectAuthenticationConfigurationSection,
                Constants.PolicyIds.SignUpUsingAppCode,
                null);

            // Configure the OpenID Connect authentication middleware for the "b2c_1a_sign_up_games_authy_code" policy that is used for
            // registering a user using an app code.
            ConfigureOpenIdConnectAuthenticationWithRedirect(
                applicationBuilder,
                Constants.PolicyIds.SignUpUsingAuthyCode,
                openIdConnectAuthenticationConfigurationSection,
                Constants.PolicyIds.SignUpUsingAuthyCode,
                null);

            // Configure the OpenID Connect authentication middleware for the "b2c_1a_step_up" policy that is used for stepping up a
            // user.
            ConfigureOpenIdConnectAuthenticationWithRedirect(
                applicationBuilder,
                Constants.PolicyIds.StepUp,
                openIdConnectAuthenticationConfigurationSection,
                Constants.PolicyIds.StepUp,
                null);

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

            services.Configure<ActivationControllerOptions>(o =>
            {
                var activationConfigurationSection = Configuration.GetSection("Activation");
                o.Key = activationConfigurationSection["Key"];
            });

            services.Configure<InvitationControllerOptions>(o =>
            {
                var invitationConfigurationSection = Configuration.GetSection("Invitation");
                o.Key = invitationConfigurationSection["Key"];
            });

            services.AddAuthorization(authorizationOptions =>
            {
                // Create an authorization policy for the Games API that requires the "Games.Read" scope to be contained in the bearer
                // JWT.
                authorizationOptions.AddPolicy(
                    Constants.AuthorizationPolicies.ReadGames,
                    policy => policy.RequireAssertion(context =>
                    {
                        var scopeClaim = context.User.FindFirst("http://schemas.microsoft.com/identity/claims/scope");

                        if (scopeClaim == null)
                        {
                            return false;
                        }

                        return scopeClaim.Value.Split(' ').Contains("Games.Read");
                    }));
            });

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(CultureFilterAttribute));
                options.Filters.Add(typeof(LocationFilterAttribute));
            });

            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            services.AddTransient<IGameRepository, GameRepository>();
            var billingService = new WingTipBillingService();
            services.AddSingleton<IBillingService>(billingService);
            var geolocationService = new FreeGeoIpGeolocationService();
            services.AddSingleton<IGeolocationService>(geolocationService);
            var musicService = new WingTipMusicService();
            services.AddSingleton<IMusicService>(musicService);
            var smtpServiceConfigurationSection = Configuration.GetSection("SendGridSmtpService");

            var smtpService = new SendGridSmtpService(
                true,
                smtpServiceConfigurationSection["Host"],
                int.Parse(smtpServiceConfigurationSection["Port"]),
                smtpServiceConfigurationSection["User"],
                smtpServiceConfigurationSection["Pass"]);

            services.AddSingleton<ISmtpService>(smtpService);
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

        private static void ConfigureOpenIdConnectAuthenticationOptions(
            IApplicationBuilder applicationBuilder,
            string authenticationScheme,
            IConfigurationSection openIdConnectAuthenticationConfigurationSection,
            string policyId,
            bool requireNonce,
            ICollection<Claim> staticPolicyClaims,
            Func<RedirectContext, IEnumerable<Claim>, Task> onRedirectToIdentityProvider)
        {
            var tenantId = openIdConnectAuthenticationConfigurationSection["TenantId"];

            var openIdConnectAuthenticationOptions = new OpenIdConnectOptions
            {
                AuthenticationScheme = authenticationScheme,
                CallbackPath = $"/{authenticationScheme}-callback", // This sign-in callback path must be registered as a redirect URI for the B2C application
                ClientId = openIdConnectAuthenticationConfigurationSection["ClientId"],
                ClientSecret = openIdConnectAuthenticationConfigurationSection["ClientSecret"],
                Events = new OpenIdConnectEvents
                {
                    OnMessageReceived = context =>
                    {
                        // If an error has been raised, then remember the return URL for use by the OnRemoteFailure event.
                        if (!string.IsNullOrEmpty(context.ProtocolMessage.Error))
                        {
                            context.HttpContext.Items["redirect_uri"] = context.Properties.RedirectUri;
                        }

                        return Task.FromResult(0);
                    },
                    OnRedirectToIdentityProvider = async context =>
                    {
                        // Set the scope parameter for the authentication request to the access permissions for the Billing and Music
                        // APIs.
                        context.ProtocolMessage.Scope = openIdConnectAuthenticationConfigurationSection["Scope"];

                        // If an Account controller action has set a UI locale (e.g. to "fr"), then set the UI locales parameter for
                        // the authentication request to this UI locale.
                        if (context.Properties.Items.ContainsKey(Constants.AuthenticationPropertiesKeys.UILocales))
                        {
                            context.ProtocolMessage.UiLocales = context.Properties.Items[Constants.AuthenticationPropertiesKeys.UILocales];
                            context.Properties.Items.Remove(Constants.AuthenticationPropertiesKeys.UILocales);
                        }

                        // If an Account controller action has set a domain hint (e.g. to "facebook.com"), then set the domain hint
                        // parameter for the authentication request to this domain hint.
                        if (context.Properties.Items.ContainsKey(Constants.AuthenticationPropertiesKeys.DomainHint))
                        {
                            context.ProtocolMessage.DomainHint = context.Properties.Items[Constants.AuthenticationPropertiesKeys.DomainHint];
                            context.Properties.Items.Remove(Constants.AuthenticationPropertiesKeys.DomainHint);
                        }

                        // Initialize a instance-level list of claims to be sent.
                        var instancePolicyClaims = new List<Claim>();

                        // If an Account controller action has set a nonce, then add the "nonce" claim to this list of claims to be
                        // sent.
                        if (context.Properties.Items.ContainsKey(Constants.AuthenticationPropertiesKeys.Nonce))
                        {
                            var nonceClaim = new Claim("nonce", context.Properties.Items[Constants.AuthenticationPropertiesKeys.Nonce]);
                            instancePolicyClaims.Add(nonceClaim);
                            context.Properties.Items.Remove(Constants.AuthenticationPropertiesKeys.Nonce);
                        }

                        // If an Account controller action has set a player profile registration mode (e.g. to "Full"), then add
                        // the "player_profile_registration_mode" claim to this list of claims to be sent.
                        if (context.Properties.Items.ContainsKey(Constants.AuthenticationPropertiesKeys.PlayerProfileRegistrationMode))
                        {
                            var playerProfileRegistrationModeClaim = new Claim("player_profile_registration_mode", context.Properties.Items[Constants.AuthenticationPropertiesKeys.PlayerProfileRegistrationMode]);
                            instancePolicyClaims.Add(playerProfileRegistrationModeClaim);
                            context.Properties.Items.Remove(Constants.AuthenticationPropertiesKeys.PlayerProfileRegistrationMode);
                        }

                        // If an Invitation controller action has set a verified email address, then add the "verified_email" claim
                        // to this list of claims to be sent.
                        if (context.Properties.Items.ContainsKey(Constants.AuthenticationPropertiesKeys.VerifiedEmail))
                        {
                            var verifiedEmailClaim = new Claim("verified_email", context.Properties.Items[Constants.AuthenticationPropertiesKeys.VerifiedEmail]);
                            instancePolicyClaims.Add(verifiedEmailClaim);
                            context.Properties.Items.Remove(Constants.AuthenticationPropertiesKeys.VerifiedEmail);
                        }

                        // Initialize the list of claims to be sent.
                        var policyClaims = new List<Claim>();

                        // Add the static-level list of claims to the list of claims to be sent.
                        if (staticPolicyClaims != null && staticPolicyClaims.Any())
                        {
                            policyClaims.AddRange(staticPolicyClaims);
                        }

                        // Add the instance-level list of claims to the list of claims to be sent.
                        if (instancePolicyClaims != null && instancePolicyClaims.Any())
                        {
                            policyClaims.AddRange(instancePolicyClaims);
                        }

                        // If a list of claims is to be sent to the target policy, then...
                        if (policyClaims != null && policyClaims.Any())
                        {
                            var configuration = await context.Options.ConfigurationManager.GetConfigurationAsync(CancellationToken.None);

                            TimeSpan policyTokenLifetime;

                            // Get the lifetime of the JSON Web Token (JWT) from the authentication session...
                            if (!context.Properties.Items.ContainsKey(Constants.AuthenticationPropertiesKeys.PolicyTokenLifetime) || !TimeSpan.TryParse(context.Properties.Items[Constants.AuthenticationPropertiesKeys.PolicyTokenLifetime], out policyTokenLifetime))
                            {
                                // ... Or set it to a default time of 5 minutes.
                                policyTokenLifetime = new TimeSpan(0, 0, 5, 0);
                            }

                            // Create the JWT containing the list of claims and signed by the client secret.
                            var selfIssuedToken = CreateSelfIssuedToken(
                                configuration.Issuer,
                                context.ProtocolMessage.RedirectUri,
                                policyTokenLifetime,
                                context.Options.ClientSecret,
                                policyClaims);

                            // Set the client assertion parameter for the authentication request to this JWT.
                            // The list of claims is received by the target policy as follows:
                            //     <InputTokenFormat>JWT</InputTokenFormat>
                            //     <CryptographicKeys>
                            //         <Key Id="client_secret" StorageReferenceId="WingTipGamesClientSecret" />
                            //     </CryptographicKeys>
                            //     <InputClaims>
                            //         <InputClaim ClaimTypeReferenceId="extension_Brand" />
                            //         <InputClaim ClaimTypeReferenceId="extension_PlayerProfileRegistrationMode" />
                            //       </InputClaims>
                            context.ProtocolMessage.Parameters.Add("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer");
                            context.ProtocolMessage.Parameters.Add("client_assertion", selfIssuedToken);
                        }

                        await onRedirectToIdentityProvider(context, policyClaims);
                    },
                    OnRemoteFailure = context =>
                    {
                        // Handle the error that is raised if the local account for a user was attempted to be linked to the social
                        // account for another user.
                        if (!string.IsNullOrEmpty(context.Failure.Message) &&
                            context.Failure.Message.Contains("server_error") &&
                            context.Failure.Message.Contains("AADB2C99001"))
                        {
                            context.Response.Redirect($"/Account/LinkError?ReturnUrl={context.HttpContext.Items["redirect_uri"]}");
                            context.HandleResponse();
                        }

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
                ProtocolValidator =
                {
                    RequireNonce = requireNonce
                },
                ResponseType = "code id_token token", // Request an access token for use with the Billing and Music APIs
                SaveTokens = true, // Save the access token to the application cookie. This is not recommended for a production application.
                SignInScheme = Constants.AuthenticationSchemes.ApplicationCookie,
                SignedOutCallbackPath = $"/{authenticationScheme}-signout-callback", // This sign-out callback path does not have to be registered as a redirect URI for the B2C application
                SignOutScheme = Constants.AuthenticationSchemes.ApplicationCookie,
            };

            // The following OpenID Connect authentication middleware allows the cross-site request forgery logic to be disabled for
            // when a policy link is sent to an end user using an email message.
            applicationBuilder.UseWingTipOpenIdConnectAuthentication(openIdConnectAuthenticationOptions);
        }

        private static void ConfigureOpenIdConnectAuthenticationWithLink(
            IApplicationBuilder applicationBuilder,
            string authenticationScheme,
            IConfigurationSection openIdConnectAuthenticationConfigurationSection,
            string policyId,
            ICollection<Claim> staticPolicyClaims)
        {
            ConfigureOpenIdConnectAuthenticationOptions(
                applicationBuilder,
                authenticationScheme,
                openIdConnectAuthenticationConfigurationSection,
                policyId,
                false,
                staticPolicyClaims,
                // Redirect to the identity provider via an email link that contains the authentication request URL.
                (context, policyClaims) =>
                {
                    context.Properties.Items.Add(OpenIdConnectDefaults.RedirectUriForCodePropertiesKey, context.ProtocolMessage.RedirectUri);
                    context.ProtocolMessage.State = context.Options.StateDataFormat.Protect(context.Properties);
                    var smtpService = context.HttpContext.RequestServices.GetRequiredService<ISmtpService>();
                    var verifiedEmailClaim = policyClaims.First(c => c.Type == "verified_email");
                    var authenticationRequestUrl = context.ProtocolMessage.CreateAuthenticationRequestUrl();
                    smtpService.SendInvitationEmail(verifiedEmailClaim.Value, authenticationRequestUrl);
                    context.HandleResponse();
                    return Task.FromResult(0);
                });
        }

        private static void ConfigureOpenIdConnectAuthenticationWithRedirect(
            IApplicationBuilder applicationBuilder,
            string authenticationScheme,
            IConfigurationSection openIdConnectAuthenticationConfigurationSection,
            string policyId,
            ICollection<Claim> staticPolicyClaims)
        {
            ConfigureOpenIdConnectAuthenticationOptions(
                applicationBuilder,
                authenticationScheme,
                openIdConnectAuthenticationConfigurationSection,
                policyId,
                true,
                staticPolicyClaims,
                // Redirect to the identity provider via a HTTP redirect that contains the authentication request URL.
                (context, policyClaims) =>
                {
                    context.Properties.Items.Add(OpenIdConnectDefaults.RedirectUriForCodePropertiesKey, context.ProtocolMessage.RedirectUri);
                    context.ProtocolMessage.State = context.Options.StateDataFormat.Protect(context.Properties);
                    var authenticationRequestUrl = context.ProtocolMessage.CreateAuthenticationRequestUrl();
                    context.Response.Redirect(authenticationRequestUrl);
                    context.HandleResponse();
                    return Task.FromResult(0);
                });
        }

        internal static string CreateSelfIssuedToken(
            string issuer,
            string audience,
            TimeSpan expiration,
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
                Expires = nowUtc.Add(expiration),
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
