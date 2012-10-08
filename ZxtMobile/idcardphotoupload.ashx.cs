using System;
using System.Collections.Generic;
using System.Web;

namespace ZxtMobile
{
    /// <summary>
    /// idcardphotoupload 的摘要说明
    /// </summary>
    public class idcardphotoupload : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            if (context.Request.Files.Count > 0)
            {
                try
                {
                    HttpPostedFile file = context.Request.Files[0];
                    file.SaveAs(System.Configuration.ConfigurationManager.AppSettings["photo"] + file.FileName);
                    context.Response.Write("s");
                }
                catch (Exception ex)
                {
                    Logger.WriteLog("page:idcardphotoupload.ashx;exception:" + ex.Message);
                    context.Response.Write("f");
                }
            }
            else
            {
                context.Response.Write("f");
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