using System.Web;
using System.Web.Routing;
using WebLibrary.Handlers;

namespace WebLibrary.RouteHandlers
{
	public class SayHelloRouteHandler : IRouteHandler
	{
		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			return new AspHandler(requestContext.RouteData.Values["Name"] as string);
		}
	}
}
