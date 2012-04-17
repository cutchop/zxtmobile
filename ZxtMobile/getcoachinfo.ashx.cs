using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

namespace ZxtMobile
{
    /// <summary>
    /// 获取教练信息
    /// </summary>
    public class getcoachinfo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string deviceID = context.Request["deviceid"];
            string session = context.Request["session"];
            string card = context.Request["card"];
            string school = context.Request["school"];

            IDataBase db = DBConfig.GetDBObjcet();
            string sql = string.Format("select * from device_info where device_id='{0}' and device_session='{1}'", deviceID, session);
            DataSet ds = null;
            try
            {
                ds = db.ExecuteReturnDataSet(sql);
            }
            catch { }
            if (ds != null && ds.Tables[0] != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    try
                    {
                        sql = string.Format("select a.*,b.coach_time from zxt_app.coach_info a left join zxt_app.coach_card b on a.coach_id=b.coach_id where a.coach_school_id='{0}' and a.coach_id='{1}'", school, card);
                        ds = db.ExecuteReturnDataSet(sql);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = ds.Tables[0].Rows[0];
                            context.Response.Write(string.Format("s|{0}|{1}|{2}|{3}|{4}", card, dr["coach_time"], dr["code"], dr["name"], dr["id_card_no"]));
                        }
                        else
                        {
                            context.Response.Write("f|教练身份验证失败");
                        }
                    }
                    catch
                    {
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