using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Data;

namespace ZxtMobile
{
    /// <summary>
    /// 指纹图片上传
    /// </summary>
    public class fingerimageupload : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            if (context.Request.Files.Count > 0)
            {
                string deviceID = context.Request["deviceid"];
                string session = context.Request["session"];
                IDataBase db = DBConfig.GetDBObjcet();
                string sql = string.Format("select * from device_info where device_id='{0}' and device_session='{1}'", deviceID, session);
                DataSet ds = null;
                try
                {
                    ds = db.ExecuteReturnDataSet(sql);
                }
                catch (Exception ex)
                {
                    Logger.WriteLog("page:photoupload.ashx;exception:" + ex.Message + ";SQL:" + sql);
                }
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count == 0)
                {
                    context.Response.Write("f|设备非法");
                }
                else
                {
                    HttpPostedFile file = context.Request.Files[0];
                    try
                    {
                        string cardno = context.Request["cardno"];
                        string n = context.Request["n"];
                        string path = System.Configuration.ConfigurationManager.AppSettings["fingerimage"];
                        if (context.Request["t"] == "1")
                        {
                            path += "coach/";
                        }
                        path = path + cardno.Substring(6) + "/";
                        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                        file.SaveAs(path + cardno + "_" + n + ".bmp");
                        context.Response.Write("s");
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog(ex.Message);
                    }
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