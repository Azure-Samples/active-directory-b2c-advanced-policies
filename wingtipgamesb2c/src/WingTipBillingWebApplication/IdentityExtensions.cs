using System;
using System.Security.Claims;
using System.Security.Principal;

namespace WingTipBillingWebApplication
{
    public static class IdentityExtensions
    {
        public static string GetDisplayName(this IIdentity identity)
        {
            return GetClaimValue(identity, ClaimTypes.Name);
        }

        private static string GetClaimValue(IIdentity identity, string claimType)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            var claimsIdentity = identity as ClaimsIdentity;
            return claimsIdentity?.FindFirst(claimType)?.Value;
        }
    }
}
