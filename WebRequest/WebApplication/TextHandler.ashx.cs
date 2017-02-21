using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication
{
    /// <summary>
    /// Summary description for TextHandler
    /// </summary>
    public class TextHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            context.Response.Write("Hello World ");
            //context.Response.Write("<img src=\"ImageHandler.ashx\" />");
            context.Response.Write("<img src=\"ImageCacheableHandler.ashx\" />");
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