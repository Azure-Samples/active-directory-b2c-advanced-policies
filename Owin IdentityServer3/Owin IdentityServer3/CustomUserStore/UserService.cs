using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services.Default;

namespace CustomUserStore
{
    public class UserService : UserServiceBase
    {
        //
        // Summary:
        //     This method gets called when the user uses an external identity provider to authenticate.
        //     The user's identity from the external provider is passed via the `externalUser`
        //     parameter which contains the provider identifier, the provider's identifier for
        //     the user, and the claims from the provider for the external user.
        //
        // Parameters:
        //   context:
        //     The context.
        public override Task AuthenticateExternalAsync(ExternalAuthenticationContext context)
        {
            var externalAuth = context.ExternalIdentity;

            // name needs to be present as claim type
            string nameClaim = string.Empty;
            var name = externalAuth.Claims.FirstOrDefault(c => string.Compare(c.Type, "name", true) == 0);
            if (name == null)
            {
                // Some dummy value
                nameClaim = "ronny";
            }
            else
            {
                nameClaim = name.Value;
            }

            var auth = new AuthenticateResult(externalAuth.ProviderId, nameClaim, externalAuth.Claims, externalAuth.Provider);
            context.AuthenticateResult = auth;
            return Task.FromResult(0);
        }


        //
        // Summary:
        //     This method is called whenever claims about the user are requested (e.g. during
        //     token creation or via the userinfo endpoint)
        //
        // Parameters:
        //   context:
        //     The context.
        public override Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject;
            List<Claim> claims = subject.Claims.ToList();

            // Just add a claim for testing
            //claims.Add(new Claim("justfortest", "testvalue"));

            context.IssuedClaims = claims;
            return Task.FromResult(0);
        }
    }
}
