using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using WebLibrary.RouteHandlers;

namespace WebLibrary
{
	public class MyApplication : HttpApplication
    {
		protected void Application_Start(object sender, EventArgs e)
		{
			RouteTable.Routes.Add("AspHandler", new Route("SayHello/{Name}",
				new RouteValueDictionary(new Dictionary<string, object> {{"Name", "World"}}),
				new SayHelloRouteHandler()));
		}

	    protected void Application_BeginRequest()
	    {
	    }
    }
}