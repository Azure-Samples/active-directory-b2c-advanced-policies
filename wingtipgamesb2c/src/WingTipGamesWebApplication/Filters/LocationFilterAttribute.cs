using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WingTipGamesWebApplication.Filters
{
    public class LocationFilterAttribute : ActionFilterAttribute
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
                var currentLocation = string.Empty;

                if (context.HttpContext.Request.Cookies.ContainsKey(Constants.CookieNames.CurrentLocation))
                {
                    currentLocation = context.HttpContext.Request.Cookies[Constants.CookieNames.CurrentLocation];
                }

                viewResult.ViewData[Constants.ViewDataKeys.CurrentLocation] = currentLocation;
            }
        }
    }
}
