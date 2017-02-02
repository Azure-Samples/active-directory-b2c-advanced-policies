using System.Web;
using System.Web.Routing;

namespace UserJourneyRecorderWebApp.Infrastructure
{
    public class UserJourneyRecorderRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new UserJourneyRecorderHttpHandler();
        }
    }
}
