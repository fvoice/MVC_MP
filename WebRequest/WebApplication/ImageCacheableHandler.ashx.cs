using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApplication
{
    /// <summary>
    /// Summary description for ImageHandler
    /// </summary>
    public class ImageCacheableHandler : IHttpHandler
    {
        private DateTime _lastModified;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "image/png";

            Image img = new Bitmap(200, 200);
            using (Graphics graphic = Graphics.FromImage(img))
            {
                graphic.FillRectangle(Brushes.Gray, 0, 0, 400, 400);
                graphic.FillEllipse(Brushes.Red, 0, 0, 400, 400);
            }
            MemoryStream imageStream = new MemoryStream();
            img.Save(imageStream, ImageFormat.Png);

            //var eTag = "computedHash";
            //if (context.Request.Headers["If-None-Match"] == eTag)
            //{
            //    context.Response.Clear();
            //    context.Response.StatusCode = (int)System.Net.HttpStatusCode.NotModified;
            //    context.Response.End();
            //}

            imageStream.WriteTo(context.Response.OutputStream);

            //cache by max-age
            //context.Response.Cache.SetCacheability(HttpCacheability.Private);
            //context.Response.Cache.SetMaxAge(new TimeSpan(0,0,30));

            //cache by eTag
            //context.Response.Cache.SetCacheability(HttpCacheability.Public);
            //context.Response.Cache.SetETag(eTag);

            //cache by Expires
            _lastModified = DateTime.Now;
            context.Response.Cache.SetLastModified(_lastModified);
            context.Response.Cache.SetExpires(DateTime.Now.AddSeconds(30));           
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