using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core;

namespace IdentityServer.Config
{
    public static class Scopes
    {
        private const string DisplayName = "displayName";
        private const string ObjectId = "objectId";


        public static IEnumerable<Scope> Get()
        {
            return new List<Scope>
                {
                    StandardScopes.OpenId,
                    StandardScopes.Profile,
                    new Scope
                    {
                        Name = "userdetails",
                        DisplayName = "userdetails",
                        Type = ScopeType.Identity,

                        Claims = new List<ScopeClaim>
                        {
                            new ScopeClaim(Constants.ClaimTypes.GivenName),
                            new ScopeClaim(Constants.ClaimTypes.FamilyName),
                            new ScopeClaim(Constants.ClaimTypes.Name),
                            new ScopeClaim(Constants.ClaimTypes.Id),
                            new ScopeClaim(ObjectId),
                            new ScopeClaim(Constants.ClaimTypes.IdentityProvider),
                            new ScopeClaim(Constants.ClaimTypes.Email)
                        },
                        IncludeAllClaimsForUser = true
                    },
                };
        }
    }
}
