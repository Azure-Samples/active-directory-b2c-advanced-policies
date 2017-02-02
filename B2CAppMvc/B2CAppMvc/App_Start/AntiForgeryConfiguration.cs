using System.Web.Helpers;

namespace B2CAppMvc
{
    public static class AntiForgeryConfiguration
    {
        public static void ConfigureAntiForgery()
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        }
    }
}
