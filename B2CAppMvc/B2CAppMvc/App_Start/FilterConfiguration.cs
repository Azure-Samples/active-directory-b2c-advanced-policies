using System.Web.Mvc;

namespace B2CAppMvc
{
    public class FilterConfiguration
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
