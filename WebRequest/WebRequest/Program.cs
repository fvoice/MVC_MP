using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace WebRequest
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://www.google.com:80/";
            HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(url);
	        //request.Method = "POST";
			request.ProtocolVersion = new Version(1,0);
	        request.AllowAutoRedirect = false;
	        request.ContentLength = 0;
            HttpWebResponse serverResponse = null;

            try
            {
                using (serverResponse = (HttpWebResponse)request.GetResponse())
                {
                    HttpStatusCode statusCode = serverResponse.StatusCode;
                    Version version = serverResponse.ProtocolVersion;
                    Console.WriteLine("{0} {1} - {2}", version,(int)statusCode, statusCode);

                    if (statusCode == HttpStatusCode.OK)
                    {
                        Console.WriteLine("Ресурс доступен");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }           

            Console.ReadLine();
        }
    }
}
