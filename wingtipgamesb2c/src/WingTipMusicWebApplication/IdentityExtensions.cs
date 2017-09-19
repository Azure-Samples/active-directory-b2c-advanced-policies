using System;
using System.Security.Claims;
using System.Security.Principal;
using Newtonsoft.Json.Linq;

namespace WingTipMusicWebApplication
{
    public static class IdentityExtensions
    {
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

        public static string GetListenerGenre(this IIdentity identity)
        {
            return GetClaimValue(identity, "listener_genre");
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
