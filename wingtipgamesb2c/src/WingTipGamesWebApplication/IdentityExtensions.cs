using System;
using System.Security.Claims;
using System.Security.Principal;
using Newtonsoft.Json.Linq;

namespace WingTipGamesWebApplication
{
    public static class IdentityExtensions
    {
        public static bool HasIdentityProvider(this IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            return !string.IsNullOrEmpty(identity.GetIdentityProvider());
        }

        public static string GetDisplayName(this IIdentity identity)
        {
            return GetClaimValue(identity, "name");
        }

        public static string GetEmailAddress(this IIdentity identity)
        {
            return GetClaimValue(identity, ClaimTypes.Email);
        }

        public static string GetIdentityProvider(this IIdentity identity)
        {
            return GetClaimValue(identity, "http://schemas.microsoft.com/identity/claims/identityprovider");
        }

        public static string GetPictureUrl(this IIdentity identity)
        {
            var pictureClaimValue = GetClaimValue(identity, "picture");

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

        public static string GetPlayerProfilePercentComplete(this IIdentity identity)
        {
            return GetClaimValue(identity, "player_profile_percent_complete");
        }

        public static string GetPlayerTag(this IIdentity identity)
        {
            return GetClaimValue(identity, "player_tag");
        }

        public static string GetPlayerZone(this IIdentity identity)
        {
            return GetClaimValue(identity, "player_zone");
        }

        public static string GetPolicy(this IIdentity identity)
        {
            return GetClaimValue(identity, "http://schemas.microsoft.com/claims/authnclassreference");
        }

        public static bool IsNewUser(this IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            var claimsIdentity = identity as ClaimsIdentity;

            if (claimsIdentity == null)
            {
                return false;
            }

            var newUserClaim = claimsIdentity.FindFirst("newUser");

            if (newUserClaim == null)
            {
                return false;
            }

            return bool.Parse(newUserClaim.Value);
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
