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
            string balance = context.Request["balance"];
            string startmi = context.Request["startmi"];
            string endmi = context.Request["endmi"];
            string subject = context.Request["subject"];
            string cardtype = "01";
            string usemi = "0";
            try
            {
                usemi = (int.Parse(endmi) - int.Parse(startmi)).ToString();
            }
            catch (Exception ex)
            {
                Logger.WriteLog("page:usedata.ashx;exception:" + ex.Message);
            }

            IDataBase db = DBConfig.GetDBObjcet();
            string sql = string.Format("select a.*,b.orgid,b.orgname from device_info a left join gmit_base.aorg b on a.belong_groupid=b.orgid where a.device_id='{0}' and a.device_session='{1}'", deviceID, session);
            DataSet ds = null;
            try
            {
                ds = db.ExecuteReturnDataSet(sql);
            }
            catch (Exception ex)
            {
                Logger.WriteLog("page:usedata.ashx;exception:" + ex.Message);
            }
            if (ds != null && ds.Tables[0] != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    try
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        if (!student.Equals(coach))
                        {
                            cardtype = "02";
                        }
                        int min = (int)Math.Ceiling((DateTime.Parse(endtime) - DateTime.Parse(starttime)).TotalMinutes);
                        sql = string.Format(@"insert into gmit_app.jx_use_data(id,school_id,card_id,coach_id,start_time,end_time,use_datetime,card_type,create_time,device_id,vehicleno,balance,group_id,group_name,begin_mi,end_mi,use_mi,use_price,subject,account_date) 
                                    values('{0}','{1}','{2}','{3}',to_date('{4}','yyyy-mm-dd hh24:mi:ss'),to_date('{5}','yyyy-mm-dd hh24:mi:ss'),{6},'{7}',sysdate,'{8}','{9}',{10},'{11}','{12}',{13},{14},{15},{16},'{17}','{18}')"
                            , guid, school, student, coach, starttime, endtime, min, cardtype, deviceID, dr["device_name"], balance, dr["orgid"], dr["orgname"], startmi, endmi, usemi, 2, subject, starttime.Substring(0, 10));
                        db.ExecuteNonQuery(sql);
                        if (cardtype == "01")
                        {
                            sql = string.Format("update gmit_app.coach_card set coach_time=coach_time-{0} where coach_id='{1}'", min, student);
                        }
                        else
                        {
                            sql = string.Format("update gmit_app.student_card set stu_yue=(stu_shichang-{0})*2,stu_shichang=stu_shichang-{0} where stu_id='{1}'", min, student);
                        }
                        db.ExecuteNonQuery(sql);
                        context.Response.Write("s");
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("page:usedata.ashx;exception:" + ex.Message);
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