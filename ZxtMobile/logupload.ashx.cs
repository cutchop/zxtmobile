using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

namespace ZxtMobile
{
    /// <summary>
    /// 设备日志上传
    /// </summary>
    public class logupload : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            if (!string.IsNullOrEmpty(context.Request["log"]))
            {
                IDataBase db = DBConfig.GetDBObjcet();
                string sql = string.Format("select * from device_info where device_id='{0}' and device_session='{1}'", context.Request["deviceid"], context.Request["session"]);
                DataSet ds = null;
                try
                {
                    ds = db.ExecuteReturnDataSet(sql);
                }
                catch (Exception ex)
                {
                    Logger.WriteLog("page:logupload.ashx;exception:" + ex.Message + ";SQL:" + sql);
                }
                if (ds != null && ds.Tables[0] != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        sql = string.Format("insert into android_client_log(device_id,log_content) values('{0}','{1}')", context.Request["deviceid"], context.Request["log"]);
                        try
                        {
                            db.ExecuteNonQuery(sql);
                            context.Response.Write("s");
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteLog("page:logupload.ashx;exception:" + ex.Message + ";SQL:" + sql);
                        }
                    }
                    else
                    {
                        context.Response.Write("设备非法");
                    }
                }
                else
                {
                    context.Response.Write("数据库异常");
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