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
    public class ImageHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "image/png";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Image img = new Bitmap(200, 200);
            using (Graphics graphic = Graphics.FromImage(img))
            {
                graphic.FillRectangle(Brushes.Black, 0, 0, 400, 400);
                graphic.FillEllipse(Brushes.Red, 0, 0, 400, 400);
            }
            MemoryStream imageStream = new MemoryStream();
            img.Save(imageStream, ImageFormat.Png);
            imageStream.WriteTo(context.Response.OutputStream);
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