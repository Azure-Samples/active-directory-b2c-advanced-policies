using System;
using System.Security.Claims;
using System.Security.Principal;

namespace WingTipToysWebApplication
{
    public static class IdentityExtensions
    {
        public static string GetDisplayName(this IIdentity identity)
        {
            return identity.Name;
        }
    }
}
