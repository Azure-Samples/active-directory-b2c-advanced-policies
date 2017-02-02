using System;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace B2CAppMvc
{
    public static class IdentityExtensions
    {
        public static string GetAuthenticationClassReference(this IIdentity identity)
        {
            return GetClaimValue(identity, "http://schemas.microsoft.com/claims/authnclassreference");
        }

        public static string GetDisplayName(this IIdentity identity)
        {
            return GetClaimValue(identity, "name");
        }

        public static string GetEmailAddress(this IIdentity identity)
        {
            return GetClaimValue(identity, "emails");
        }

        public static string GetGamerTag(this IIdentity identity)
        {
            return GetClaimValue(identity, "extension_GamerTag");
        }

        public static string GetGamerZone(this IIdentity identity)
        {
            return GetClaimValue(identity, "extension_GamerZone");
        }

        public static string GetGivenName(this IIdentity identity)
        {
            return GetClaimValue(identity, ClaimTypes.GivenName);
        }

        public static string GetLoyaltyNumber(this IIdentity identity)
        {
            return GetClaimValue(identity, "Loyalty");
        }

        public static string GetPictureUrl(this IIdentity identity)
        {
            var pictureClaimValue = GetClaimValue(identity, "extension_picture");

            if (!string.IsNullOrEmpty(pictureClaimValue))
            {
                var picture = JObject.Parse(pictureClaimValue);
                var pictureData = picture["data"];
                var isPictureSilhouette = (bool)pictureData["is_silhouette"];

                if (!isPictureSilhouette)
                {
                    return (string)pictureData["url"];
                }
            }

            return "/images/silhouette-picture.png";
        }

        public static string GetSurname(this IIdentity identity)
        {
            return GetClaimValue(identity, ClaimTypes.Surname);
        }

        public static bool IsEmployee(this IIdentity identity)
        {
            var identityProviderValue = GetClaimValue(identity, "http://schemas.microsoft.com/identity/claims/identityprovider");
            return identityProviderValue == "OpenIdConnect";
        }

        private static string GetClaimValue(IIdentity identity, string claimType)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            var claimsIdentity = identity as ClaimsIdentity;
            return claimsIdentity?.FindFirstValue(claimType);
        }
    }
}
