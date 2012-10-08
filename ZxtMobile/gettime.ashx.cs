using System;
using System.Collections.Generic;
using System.Web;

namespace ZxtMobile
{
    /// <summary>
    /// 获取服务器时间
    /// </summary>
    public class gettime : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(Convert.ToInt64(DateTime.Now.Subtract(DateTime.Parse("1970-01-01 08:00:00")).TotalMilliseconds));
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