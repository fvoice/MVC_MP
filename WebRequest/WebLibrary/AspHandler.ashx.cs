using System.Web;

namespace WebLibrary
{
    /// <summary>
	/// Summary description for AspHandler
    /// </summary>
    public class AspHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            context.Response.Write("Hello World !!!");
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