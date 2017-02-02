using System;
using System.Web;
using System.Web.Routing;
using UserJourneyRecorderWebApp.Infrastructure;

namespace UserJourneyRecorderWebApp
{
    public class UserJourneyRecorderWebApplication : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var routeHandler = new UserJourneyRecorderRouteHandler();
            var route = new Route("stream", routeHandler);
            RouteTable.Routes.Add(string.Empty, route);
        }
    }
}
