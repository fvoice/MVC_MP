using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TaskProject2_HttpListener
{
	class Program
	{
		private static readonly HttpListener Listener = new HttpListener();

		private static string _siteDirectory = "c:\\Sites\\636232874854173930\\";

		public static void Main()
		{
			Listener.Prefixes.Add("http://+:80/");
			Listener.Prefixes.Add("http://simpleserver.org:80/");
			Listener.Start();
			Listen();
			Console.WriteLine("Listening...");
			Console.WriteLine("Press any key to exit...");
			Console.ReadKey();
		}

		private static async void Listen()
		{
			while (true)
			{
				var context = await Listener.GetContextAsync();
				Console.WriteLine("Request for {0}", context.Request.Url.AbsoluteUri);
				await Task.Factory.StartNew(() => ProcessRequest(context));
			}
		}

		private static void ProcessRequest(HttpListenerContext context)
		{
			HttpListenerResponse response = context.Response;
			Stream output = response.OutputStream;

			response.Headers.Add("Server", "My HttpListener server");

			var path = string.Format("{0}{1}", _siteDirectory, context.Request.Url.LocalPath == "/" ? "index.html" : context.Request.Url.LocalPath.Replace("/", "\\"));
			if (!File.Exists(path))
			{
				response.StatusCode = (int)HttpStatusCode.NotFound;
				response.StatusDescription = HttpStatusCode.NotFound.ToString();
				var error = Encoding.UTF8.GetBytes("Sorry, that page does not exist");
				output.Write(error, 0, error.Length);
				output.Close();

				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Page {0} does not exist", context.Request.Url.AbsoluteUri);
				Console.ForegroundColor = ConsoleColor.Green;

				return;
			}
			var fileName = Path.GetFileName(path);
			string mimeType = MimeMapping.GetMimeMapping(fileName);
			response.AddHeader("Content-type", mimeType);

			var bytes = File.ReadAllBytes(path);
			output.Write(bytes, 0, bytes.Length);
			output.Close();	

			Console.WriteLine("Response sended");
		}
	}
}
