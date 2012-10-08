using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

namespace ZxtMobile
{
    /// <summary>
    /// 记录日志
    /// </summary>
    public class log : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            if (!string.IsNullOrEmpty(context.Request["imei"]))
            {
                IDataBase db = DBConfig.GetDBObjcet();
                string sql = string.Format("select device_id from device_info where device_imei='{0}'", context.Request["imei"]);
                try
                {
                    DataSet ds = db.ExecuteReturnDataSet(sql);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        sql = string.Format("insert into device_work_log(device_id,log_type,device_imei) values('{0}','{1}','{2}')", ds.Tables[0].Rows[0]["device_id"], context.Request["c"], context.Request["imei"]);
                        db.ExecuteNonQuery(sql);
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLog("page:log.ashx;exception:" + ex.Message + ";SQL:" + sql);
                    context.Response.Write("failure|数据库异常");
                }
            }
            context.Response.Write("ok");
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