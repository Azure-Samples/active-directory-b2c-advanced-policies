using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WingTipToysWebApplication.HtmlHelpers
{
    public static class NavbarHelper
    {
        public static string ActiveNavbarItem(this IHtmlHelper htmlHelper, string expectedController, string expectedAction)
        {
            if (htmlHelper == null)
            {
                throw new ArgumentNullException("htmlHelper");
            }

            var actualController = htmlHelper.ViewContext.RouteData.Values["controller"].ToString();

            if (!actualController.Equals(expectedController, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var actualAction = htmlHelper.ViewContext.RouteData.Values["action"].ToString();

            if (!actualAction.Equals(expectedAction, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return "active";
        }
    }
}
