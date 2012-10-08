using System;
using System.Collections.Generic;
using System.Web;
using System.IO;

namespace ZxtMobile
{
    /// <summary>
    /// 训练照片
    /// </summary>
    public class trainphoto : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "image/pjpeg";
                string deviceid = context.Request["d"];
                string time = context.Request["t"];
                string n = context.Request["n"];
                if (!string.IsNullOrEmpty(deviceid) && !string.IsNullOrEmpty(time) && !string.IsNullOrEmpty(n))
                {
                    string file = System.Configuration.ConfigurationManager.AppSettings["photosave"] + time.Substring(0, 8) + "/" + deviceid + "/" + time + "_" + n + ".jpg";
                    if (File.Exists(file))
                    {
                        context.Response.WriteFile(file);
                    }
                    else
                    {
                        context.Response.WriteFile(System.Configuration.ConfigurationManager.AppSettings["photosave"] + "0.png");
                    }
                }
                else
                {
                    context.Response.WriteFile(System.Configuration.ConfigurationManager.AppSettings["photosave"] + "0.png");
                }
            }
            catch { return; }
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