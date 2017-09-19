using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace WingTipCommon.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AuthenticationContext _authenticationContext;

        public AuthenticationService(string tenantId)
        {
            if (string.IsNullOrEmpty(tenantId))
            {
                throw new ArgumentNullException(nameof(tenantId));
            }

            _authenticationContext = new AuthenticationContext($"https://login.microsoftonline.com/{tenantId}");
        }

        public Task<AuthenticationResult> AcquireTokenAsync(string resource, ClientCredential clientCredential)
        {
            return _authenticationContext.AcquireTokenAsync(resource, clientCredential);
        }

        public Task<AuthenticationResult> AcquireTokenAsync(
            string resource,
            string clientId,
            Uri redirectUri,
            string userId)
        {
            return _authenticationContext.AcquireTokenAsync(
                resource,
                clientId,
                redirectUri,
                new PlatformParameters(PromptBehavior.Never),
                new UserIdentifier(userId, UserIdentifierType.UniqueId));
        }

        public Task<AuthenticationResult> AcquireTokenByAuthorizationCodeAsync(
            string authorizationCode,
            Uri redirectUri,
            ClientCredential clientCredential,
            string resource)
        {
            return _authenticationContext.AcquireTokenByAuthorizationCodeAsync(
                authorizationCode,
                redirectUri,
                clientCredential,
                resource);
        }
    }
}
