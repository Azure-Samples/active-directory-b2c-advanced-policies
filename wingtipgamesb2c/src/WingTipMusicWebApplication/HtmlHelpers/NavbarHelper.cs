using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WingTipMusicWebApplication.HtmlHelpers
{
    public static class NavbarHelper
    {
        public static string ActiveNavbarItem(this IHtmlHelper htmlHelper, string expectedController)
        {
            if (htmlHelper == null)
            {
                throw new ArgumentNullException("htmlHelper");
            }

            var actualController = htmlHelper.ViewContext.RouteData.Values["controller"].ToString();

            if (actualController.Equals(expectedController, StringComparison.OrdinalIgnoreCase))
            {
                return "active";
            }

            return null;
        }
    }
}
