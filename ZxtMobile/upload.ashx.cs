using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

namespace ZxtMobile
{
    /// <summary>
    /// 上传数据
    /// </summary>
    public class upload : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string deviceID = context.Request["deviceid"];
            string session = context.Request["session"];
            string lng = context.Request["lng"];
            string lat = context.Request["lat"];
            string mode = context.Request["mode"];
            string coach = context.Request["coach"];
            string student = context.Request["student"];
            string starttime = context.Request["starttime"];
            string balance = context.Request["balance"];
            string subject = context.Request["subject"];

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
                        if (double.Parse(lng) == 0 && double.Parse(lat) == 0)//没有经纬度时使用上一次的值
                        {
                            sql = string.Format("insert into device_his_track(device_id,lon,lat,v_lon,v_lat,logintime) select device_id,lon,lat,lon as v_lon,lat as v_lat,sysdate as logintime from device_state where device_id='{0}'", deviceID);
                            db.ExecuteNonQuery(sql);
                            sql = string.Format("update device_state set logintime=sysdate where device_id='{0}'", deviceID);
                            db.ExecuteNonQuery(sql);
                        }
                        else
                        {
                            sql = string.Format("insert into device_his_track(device_id,lon,lat,v_lon,v_lat,logintime) values('{0}',{1},{2},{3},{4},sysdate)", deviceID, lng, lat, lng, lat);
                            db.ExecuteNonQuery(sql);
                            sql = string.Format("update device_state set lon={0},lat={1},logintime=sysdate where device_id='{2}'", lng, lat, deviceID);
                            db.ExecuteNonQuery(sql);
                        }
                        if (!string.IsNullOrEmpty(student))
                        {
                            try
                            {
                                if (student.Equals(coach))
                                {
                                    student = "1" + student;
                                }
                                else
                                {
                                    student = "2" + student;
                                }
                                sql = string.Format("update zxt_app.jx_device_status set mode_type='{0}',last_update_time=sysdate,cur_coach_ic_id='{1}',cur_stu_ic_id='{2}',cur_stu_starttime={3},cur_stu_balance={4},subject='{5}' where device_id='{6}'"
                                    , mode == "1" ? "非计费模式" : "计费模式", coach, student, string.IsNullOrEmpty(starttime) ? "null" : "to_date('" + starttime + "','yyyy-mm-dd hh24:mi:ss')", string.IsNullOrEmpty(balance) ? "null" : balance, subject, deviceID);
                                db.ExecuteNonQuery(sql);
                            }
                            catch { }
                        }
                        context.Response.Write("success");
                    }
                    catch
                    {
                        context.Response.Write("数据库异常" + sql);
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