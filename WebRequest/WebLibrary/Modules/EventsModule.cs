using System;
using System.Web;
using WebLibrary.Handlers;

namespace WebLibrary.Modules
{
	public class EventsModule : IHttpModule
	{
		public void Init(HttpApplication context)
		{
			context.BeginRequest += context_BeginRequest;
			context.EndRequest += context_EndRequest;
			context.MapRequestHandler += context_MapRequestHandler;
			context.PostMapRequestHandler += context_PostMapRequestHandler;
		}

		void context_BeginRequest(object sender, EventArgs e)
		{
			HttpApplication application = (HttpApplication)sender;
			HttpContext context = application.Context;
			context.Response.Write("<H2>Begin request processing</H2>");
		}

		private void context_MapRequestHandler(object sender, EventArgs e)
		{
			var handler = ((HttpApplication)(sender)).Context.Handler;
		}

		private void context_PostMapRequestHandler(object sender, EventArgs e)
		{
			var handler = ((HttpApplication)(sender)).Context.Handler;
			//((HttpApplication)(sender)).Context.Handler = new CounterHandler();
		}

		void context_EndRequest(object sender, EventArgs e)
		{
			HttpApplication application = (HttpApplication)sender;
			HttpContext context = application.Context;
			context.Response.Write("<H2>End request processing</H2>");
		}

		public void Dispose()
		{
		}
	}
}
