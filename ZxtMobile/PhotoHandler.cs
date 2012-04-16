using System;
using System.Web;
using System.Text;
using System.IO;

namespace ZxtMobile
{
    public class PhotoHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string filename = System.Configuration.ConfigurationManager.AppSettings["photo"] + context.Request.FilePath.Substring(context.Request.FilePath.LastIndexOf("/") + 1);
                if (File.Exists(filename))
                {
                    context.Response.ContentType = "image/pjpeg";
                    context.Response.WriteFile(filename);
                }
            }
            catch { return; }
        }
    }
}
