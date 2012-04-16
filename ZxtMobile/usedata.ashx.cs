using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

namespace ZxtMobile
{
    /// <summary>
    /// 上传训练数据
    /// </summary>
    public class usedata : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string deviceID = context.Request["deviceid"];
            string session = context.Request["session"];
            string school = context.Request["school"];
            string guid = context.Request["guid"];
            string coach = context.Request["coach"];
            string student = context.Request["student"];
            string starttime = context.Request["starttime"];
            string endtime = context.Request["endtime"];
            string cardtype = "01";

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
                            if (!student.Equals(coach))
                            {
                                cardtype = "02";
                            }
                            int min = (int)Math.Ceiling((DateTime.Parse(endtime) - DateTime.Parse(starttime)).TotalMinutes);
                            sql = string.Format("insert into zxt_app.jx_use_data(id,school_id,card_id,coach_id,start_time,end_time,use_datetime,card_type,create_time,device_id) values('{0}','{1}','{2}','{3}',to_date('{4}','yyyy-mm-dd hh24:mi:ss'),to_date('{5}','yyyy-mm-dd hh24:mi:ss'),{6},'{7}',sysdate,'{8}')"
                                , guid, school, student, coach, starttime, endtime, min, cardtype, deviceID);
                            db.ExecuteNonQuery(sql);
                        context.Response.Write("s");
                    }
                    catch
                    {
                        context.Response.Write("数据库异常");
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

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}