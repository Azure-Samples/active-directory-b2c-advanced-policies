using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using B2CAppMvc.Models;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security.OpenIdConnect;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace B2CAppMvc
{
    public class B2COpenidConnect : OpenIdConnectAuthenticationOptions
    {
        /// <summary>
        /// Property to keep track of open id connect options
        /// </summary>
        private OpenIdConnectAuthenticationOptions options;

        /// <summary>
        /// Property to keep track of open id connect options
        /// </summary>
        private ClientClaims clientClaims;

        /// <summary>
        /// Get the open id connect options
        /// </summary>
        public OpenIdConnectAuthenticationOptions Options
        {
            get
            {
                return options;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="B2COpenidConnect"/> class.
        /// </summary>
        /// <param name="name">Name of the policy</param>
        /// <param name="tenant">B2C tenant</param>
        /// <param name="policy">B2C policy</param>
        /// <param name="clientId">Client id to connect to B2C App</param>
        /// <param name="clientSecret">Client secret to autneticate to B2C App</param>
        /// <param name="metadataUri">B2C metadata endpoint</param>
        /// <param name="redirectUri">Redirect Uri from B2C</param>
        /// <param name="addClientClaimsToPolicy">Defines policy and claims to add to this policy </param>
        public B2COpenidConnect(string name, string tenant, string policy, string clientId, string clientSecret, string metadataUri, string redirectUri, string addClientClaimsToPolicy = null)
        {
            string caption = string.Format(name, policy);
            if (!string.IsNullOrWhiteSpace(addClientClaimsToPolicy))
            {
                this.clientClaims = JsonConvert.DeserializeObject<ClientClaims>(addClientClaimsToPolicy);
            }

            options = new OpenIdConnectAuthenticationOptions
            {
                Notifications = new OpenIdConnectAuthenticationNotifications()
                {
                    AuthenticationFailed = context =>
                    {
                        context.HandleResponse();

                        if (context.ProtocolMessage.Error == "access_denied" && context.ProtocolMessage.ErrorDescription.StartsWith("AADB2C90118"))
                        {
                            context.Response.Redirect("/Account/ResetPassword");
                        }
                        else if (context.ProtocolMessage.Error == "access_denied" && context.ProtocolMessage.ErrorDescription.StartsWith("AADB2C90091"))
                        {
                            context.Response.Redirect("/Home/Index");
                        }
                        else
                        {
                            context.Response.Redirect("/Error/Index?error=" + context.ProtocolMessage.Error + "&error_description=" + context.ProtocolMessage.ErrorDescription);
                        }

                        return Task.FromResult(0);
                    },
                    RedirectToIdentityProvider = async context =>
                    {
                        // The open id class can't deal with authorization uri which already contain '?'
                        // We need this work around to cover it in the request
                        var parts = context.ProtocolMessage.IssuerAddress.Split(new[] { '?' });
                        context.ProtocolMessage.IssuerAddress = parts[0];
                        if (parts.Length > 1)
                        {
                            context.ProtocolMessage.Parameters.Add("p", policy);
                        }


                        if (context.ProtocolMessage.RequestType != OpenIdConnectRequestType.LogoutRequest)
                        {
                            // Check to add client auth to request
                            if (this.clientClaims != null && string.Compare(policy, clientClaims.Policy, true) == 0)
                            {
                                List<Claim> claims = new List<Claim>
                                {
                                    new Claim(clientClaims.ClaimType, clientClaims.ClaimValue),
                                    new Claim("state", context.ProtocolMessage.Parameters["state"])
                                };

                                OpenIdConnectConfiguration conf = await this.GetConfiguration(context.OwinContext, context.Options);
                                string jwt = ClientAuthToken(claims, conf.Issuer, redirectUri, clientSecret);

                                // Add client auth assertion
                                context.ProtocolMessage.Parameters.Add("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer");
                                context.ProtocolMessage.Parameters.Add("client_assertion", jwt);
                            }
                        }

                        await Task.FromResult(0);
                        
                    },
                    SecurityTokenValidated = context =>
                    {
                        Console.WriteLine("B2C returned JWT : '{0}'", context.ProtocolMessage.IdToken);
                        return Task.FromResult(0);
                    }
                },
                TokenValidationParameters = new TokenValidationParameters
                {
                    SaveSigninToken = true
                },
                UseTokenLifetime = true,
                Caption = caption,
                AuthenticationType = policy,
                Scope = "openid",
                ResponseType = "id_token",
                ClientId = clientId,
                MetadataAddress = string.Format(metadataUri, tenant, policy),
                ClientSecret = clientSecret,
                RedirectUri = redirectUri,
                PostLogoutRedirectUri = redirectUri
            };
        }

        /// <summary>
        /// Create client auth jwt assertion
        /// </summary>
        /// <param name="claims">List of claims to be put into the assertion</param>
        /// <param name="issuer">Issuer claim of the assertion</param>
        /// <param name="audience">Audience claim of the assertion</param>
        /// <param name="clientSecret">Client secret used to sign the assertion</param>
        /// <returns></returns>
        public static string ClientAuthToken(List<Claim> claims, string issuer, string audience, string clientSecret)
        {
            if (string.IsNullOrWhiteSpace(issuer))
                throw new ArgumentNullException("issuer");

            if (string.IsNullOrWhiteSpace(audience))
                throw new ArgumentNullException("audience");

            if (string.IsNullOrWhiteSpace(clientSecret))
                throw new ArgumentNullException("clientSecret");

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            InMemorySymmetricSecurityKey key = new InMemorySymmetricSecurityKey(Encoding.UTF8.GetBytes(clientSecret));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                // Create the token descriptor with empty claims since the interface only allows string values. Then addd the claims
                // to the payload later (below).
                Subject = new ClaimsIdentity(claims),
                Lifetime = new System.IdentityModel.Protocols.WSTrust.Lifetime(DateTime.Now, DateTime.Now.AddDays(1)),
                SigningCredentials = creds,
                AppliesToAddress = audience,
                TokenIssuerName = issuer                
            };

            JwtSecurityToken token = (JwtSecurityToken)tokenHandler.CreateToken(tokenDescriptor);
            string jwt = tokenHandler.WriteToken(token);
            return jwt;
        }

        /// <summary>
        /// Get claims from a validated id_token. Don't validate the token again but just extract the claims.
        /// </summary>
        /// <param name="id_token">Id token</param>
        /// <returns>List of <see cref="Claim"/></returns>
        public static List<Claim> GetClaims(string id_token)
        {
            if (string.IsNullOrWhiteSpace(id_token))
            {
                return null;
            }

            // Retrieve claims from verified token
            string[] parts = id_token.Split(new char[] { '.' });
            if (parts.Length != 3)
                return null;

            // Convert base64 url to claims            
            string base64 = parts[1].PadRight(parts[1].Length + (4 - parts[1].Length % 4) % 4, '=');
            base64 = base64.Replace('-', '+').Replace('_', '/');
            string claims = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
            JObject jsonClaim = JObject.Parse(claims);
            Dictionary<string, object> values = JsonConvert.DeserializeObject<Dictionary<string, object>>(claims);

            List<Claim> tokenClaims = new List<Claim>();
            foreach(var key in values.Keys)
            {
                if (values[key] is string)
                {
                    tokenClaims.Add(new Claim(key, values[key] as string));
                }
            }

            return tokenClaims;
        }

        /// <summary>
        /// Get openid connect configuration
        /// </summary>
        /// <param name="context">Owin context</param>
        /// <param name="options">Openid connect options</param>
        /// <returns></returns>
        private async Task<OpenIdConnectConfiguration> GetConfiguration(IOwinContext context, OpenIdConnectAuthenticationOptions options)
        {
            return await options.ConfigurationManager.GetConfigurationAsync(context.Request.CallCancelled);
        }
    }
}