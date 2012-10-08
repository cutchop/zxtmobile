using System;
using System.Collections.Generic;
using System.Web;

namespace ZxtMobile
{
    /// <summary>
    /// 获取指纹
    /// </summary>
    public class getfinger : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string cardno = context.Request["cardno"];
            string n = context.Request["n"];
            if (!string.IsNullOrEmpty(cardno) && !string.IsNullOrEmpty(n))
            {
                string filename = System.Configuration.ConfigurationManager.AppSettings["finger"];
                if (context.Request["t"] == "1")
                {
                    filename += "coach/";
                }
                filename = filename +cardno.Substring(6) + "/" + cardno + "_" + n;
                if (System.IO.File.Exists(filename))
                {
                    context.Response.WriteFile(filename);
                }
                else
                {
                    context.Response.Write("no");
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