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
            double lng,lat,v_lng,v_lat;
            try
            {
                lng = v_lng = double.Parse(context.Request["lng"]);
            }
            catch
            {
                lng = v_lng = 0;
            }
            try
            {
                lat = v_lat = double.Parse(context.Request["lat"]);
            }
            catch
            {
                lat = v_lat = 0;
            }
            int senspeed = 0;
            try
            {
                senspeed = int.Parse(context.Request["senspeed"]);
            }
            catch
            {
                senspeed = 0;
            }
            string speed = context.Request["speed"];
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
            catch(Exception ex)
            {
                Logger.WriteLog("page:upload.ashx;exception:" + ex.Message + ";SQL:" + sql);
            }
            if (ds != null && ds.Tables[0] != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    try
                    {
                        if (lng == 0 && lat == 0)//没有经纬度时使用上一次的值
                        {
                            sql = string.Format("insert into device_his_track(device_id,lon,lat,v_lon,v_lat,speed,sen_speed,logintime) select device_id,lon,lat,v_lon,v_lat,speed,{0} as senspeed,sysdate as logintime from device_state where device_id='{1}'", senspeed, deviceID);
                            db.ExecuteNonQuery(sql);
                            sql = string.Format("update device_state set logintime=sysdate,cur_dev_ip='{0}',sen_speed={1} where device_id='{2}'", context.Request.UserHostAddress, senspeed, deviceID);
                            db.ExecuteNonQuery(sql);
                        }
                        else
                        {
                            sql = string.Format("select get_offsetx({0},{1}) from dual", lng, lat);
                            ds = db.ExecuteReturnDataSet(sql);
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                try
                                {
                                    v_lng = lng + double.Parse(ds.Tables[0].Rows[0][0].ToString().Split('|')[0]);
                                }
                                catch { }
                                try
                                {
                                    v_lat = lat + double.Parse(ds.Tables[0].Rows[0][0].ToString().Split('|')[1]);
                                }
                                catch { }
                            }
                            sql = string.Format("insert into device_his_track(device_id,lon,lat,v_lon,v_lat,speed,sen_speed,logintime) values('{0}',{1},{2},{3},{4},{5},{6},sysdate)", deviceID, lng, lat, v_lng, v_lat, speed, senspeed);
                            db.ExecuteNonQuery(sql);
                            string status = "行驶";
                            if (string.IsNullOrEmpty(speed) || speed.Equals("0"))
                            {
                                status = "停车";
                            }
                            sql = string.Format("update device_state set lon={0},lat={1},v_lon={2},v_lat={3},speed={4},sen_speed={5},logintime=sysdate,device_status='{6}',cur_status='{7}',cur_dev_ip='{8}' where device_id='{9}'", lng, lat, v_lng, v_lat, speed, senspeed, "正常", status, context.Request.UserHostAddress, deviceID);
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
                                sql = string.Format("update gmit_app.jx_device_status set mode_type='{0}',last_update_time=sysdate,cur_coach_ic_id='{1}',cur_stu_ic_id='{2}',cur_stu_starttime={3},cur_stu_balance={4},subject='{5}' where device_id='{6}'"
                                    , mode == "1" ? "非计费模式" : "计费模式", coach, student, string.IsNullOrEmpty(starttime) ? "null" : "to_date('" + starttime + "','yyyy-mm-dd hh24:mi:ss')", string.IsNullOrEmpty(balance) ? "null" : balance, subject, deviceID);
                                db.ExecuteNonQuery(sql);
                            }
                            catch (Exception ex)
                            {
                                Logger.WriteLog("page:upload.ashx;exception:" + ex.Message + ";SQL:" + sql);
                            }
                        }
                        context.Response.Write("s");
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("page:upload.ashx;exception:" + ex.Message + ";SQL:" + sql);
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