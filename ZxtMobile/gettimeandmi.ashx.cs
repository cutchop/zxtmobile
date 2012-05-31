using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

namespace ZxtMobile
{
    /// <summary>
    /// 获取学时和里程
    /// </summary>
    public class gettimeandmi : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string deviceID = context.Request["deviceid"];
            string session = context.Request["session"];
            string stuid = context.Request["stuid"];
            string school = context.Request["school"];

            IDataBase db = DBConfig.GetDBObjcet();
            string sql = string.Format("select * from device_info where device_id='{0}' and device_session='{1}'", deviceID, session);
            DataSet ds = null;
            try
            {
                ds = db.ExecuteReturnDataSet(sql);
            }
            catch (Exception ex)
            {
                Logger.WriteLog("page:gettimeandmi.ashx;exception:" + ex.Message + ";SQL:" + sql);
            }
            if (ds != null && ds.Tables[0] != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    try
                    {
                        sql = string.Format("select sum(use_datetime) as usetime,sum(use_mi) as usemi from gmit_app.jx_use_data where card_id in (select self_18 from gmit_app.student_info where school_id='{0}' and stuinfo_id='{1}')", school, stuid);
                        ds = db.ExecuteReturnDataSet(sql);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = ds.Tables[0].Rows[0];
                            context.Response.Write(string.Format("s|{0}|{1}", dr["usetime"], dr["usemi"]));
                        }
                        else
                        {
                            context.Response.Write("s|0|0");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("page:gettimeandmi.ashx;exception:" + ex.Message + ";SQL:" + sql);
                        context.Response.Write("f|数据库异常");
                    }
                }
                else
                {
                    context.Response.Write("f|设备非法");
                }
            }
            else
            {
                context.Response.Write("f|数据库异常");
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