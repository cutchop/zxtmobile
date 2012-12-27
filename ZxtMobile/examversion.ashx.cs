using System;
using System.Collections.Generic;
using System.Web;

namespace ZxtMobile
{
    /// <summary>
    /// version 的摘要说明
    /// </summary>
    public class examversion : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("s|" + System.Configuration.ConfigurationManager.AppSettings["examVersion"]);
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