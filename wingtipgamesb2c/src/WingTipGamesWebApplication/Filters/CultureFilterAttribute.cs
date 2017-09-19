using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WingTipGamesWebApplication.Filters
{
    public class CultureFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            base.OnActionExecuted(context);
            var viewResult = context.Result as ViewResult;

            if (viewResult != null)
            {
                var currentCulture = string.Empty;

                if (context.HttpContext.Request.Cookies.ContainsKey(Constants.CookieNames.CurrentLocale))
                {
                    currentCulture = context.HttpContext.Request.Cookies[Constants.CookieNames.CurrentLocale];
                }

                viewResult.ViewData[Constants.ViewDataKeys.CurrentCulture] = currentCulture;
            }
        }
    }
}
