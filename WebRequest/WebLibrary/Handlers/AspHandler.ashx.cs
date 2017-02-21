using System.Web;

namespace WebLibrary.Handlers
{
    /// <summary>
	/// Summary description for AspHandler
    /// </summary>
    public class AspHandler : IHttpHandler
    {
	    public AspHandler()
	    {
	    }

	    private readonly string _text;
		public AspHandler(string text)
		{
			_text = text;
		}

	    public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            context.Response.Write(string.Format("<H1>Hello {0}!!!</H1>", string.IsNullOrEmpty(_text) ? "World" : _text));
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