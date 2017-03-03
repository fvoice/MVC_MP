using System;
using System.Web;
using System.Web.SessionState;

namespace WebLibrary.Handlers
{
	public class CounterHandler : IHttpHandler, IRequiresSessionState
	{
		private string key = "CounterKey";

		public void ProcessRequest(HttpContext context)
		{
			if (context.Session[key] == null) context.Session[key] = 0;
			var val = (int)context.Session[key];
			context.Session[key] = val + 1;

			context.Response.ContentType = "text/html";
			context.Response.Write(string.Format("<H1>Current number is {0}</H1>", val));
			context.Response.Write(string.Format("<H1>Domain id is {0}</H1>", AppDomain.CurrentDomain.Id));
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}
