using System;
using System.Collections.Generic;
using System.Web;
using System.Drawing;

namespace ZxtMobile
{
    /// <summary>
    /// photo 的摘要说明
    /// </summary>
    public class photo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if (!string.IsNullOrEmpty(context.Request["id"]))
            {
                string filename = System.Configuration.ConfigurationManager.AppSettings["photo"] + context.Request["id"] + ".bmp";
                if (System.IO.File.Exists(filename))
                {                    
                    //Bitmap result = new Bitmap(filename);
                    //context.Response.ContentType = "image/pjpeg";
                    //context.Response.AppendHeader("content-disposition", "filename=photo.jpg");
                    //result.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    context.Response.WriteFile(filename);
                }
            }
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