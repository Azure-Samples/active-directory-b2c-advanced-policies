using System.Collections.Generic;
using System.Security.Claims;

namespace WingTipBillingWebApplication.Identity
{
    public static class ClaimsPrincipalHelper
    {
        public static ClaimsPrincipal Transform(ClaimsPrincipal principal)
        {
            if (principal.Identity.IsAuthenticated)
            {
                var claims = new List<Claim>();
                claims.AddRange(principal.Claims);

                var identity = new ClaimsIdentity(
                    claims,
                    principal.Identity.AuthenticationType,
                    ClaimTypes.Name,
                    ClaimTypes.Role)
                {
                    BootstrapContext = ((ClaimsIdentity)principal.Identity).BootstrapContext
                };

                return new ClaimsPrincipal(identity);
            }

            return principal;
        }
    }
}
