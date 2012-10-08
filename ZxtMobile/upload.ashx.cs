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
            string taskid = context.Request["taskid"];
            if (taskid == null) taskid = "";
            string logintime = context.Request["logintime"];
            string acc = context.Request["acc"];
            string histable = "";
            if (string.IsNullOrEmpty(logintime))
            {
                histable = "lbshis_" + DateTime.Now.ToString("yyyyMMdd") + ".device_his_track_" + deviceID.Substring(deviceID.Length - 2);
                logintime = "sysdate";
            }
            else
            {
                histable = "lbshis_" + logintime.Substring(0, 10).Replace("-", "") + ".device_his_track_" + deviceID.Substring(deviceID.Length - 2);
                //if (deviceID == "13333333333")
                //{
                //    logintime = "to_date('" + DateTime.Parse(logintime).AddHours(8).ToString("yyyy-MM-dd HH:mm:ss") + "','yyyy-mm-dd hh24:mi:ss')";
                //}
                //else
                //{
                    logintime = "to_date('" + logintime + "','yyyy-mm-dd hh24:mi:ss')";
                //}
            }
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
                            sql = string.Format("insert into {0}(device_id,lon,lat,v_lon,v_lat,speed,sen_speed,logintime,task_id) select device_id,lon,lat,v_lon,v_lat,speed,{1} as senspeed,{2} as logintime,'{3}' as task_id from device_state where device_id='{4}'", histable, senspeed, logintime, taskid, deviceID);
                            db.ExecuteNonQuery(sql);
                            sql = string.Format("update device_state set cur_dev_ip='{0}',sen_speed={1},last_updatetime=sysdate where device_id='{2}'", context.Request.UserHostAddress, senspeed, deviceID);
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
                            sql = string.Format("insert into {0}(device_id,lon,lat,v_lon,v_lat,speed,sen_speed,logintime,task_id) values('{1}',{2},{3},{4},{5},{6},{7},{8},'{9}')", histable, deviceID, lng, lat, v_lng, v_lat, speed, senspeed, logintime, taskid);
                            db.ExecuteNonQuery(sql);
                            string status = "行驶";
                            if (acc == "0")
                            {
                                status = "熄火";
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(speed) || speed.Equals("0"))
                                {
                                    status = "停车";
                                }
                            }
                            sql = string.Format("update device_state set lon={0},lat={1},v_lon={2},v_lat={3},speed={4},sen_speed={5},logintime={6},device_status='{7}',cur_status='{8}',cur_dev_ip='{9}',last_updatetime=sysdate,acc='{10}' where device_id='{11}'", lng, lat, v_lng, v_lat, speed, senspeed, logintime, "正常", status, context.Request.UserHostAddress, acc, deviceID);
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
                        else
                        {
                            try
                            {
                                sql = string.Format("update gmit_app.jx_device_status set last_update_time=sysdate,cur_stu_ic_id='',cur_stu_starttime=null,cur_stu_balance=null where device_id='{0}'", deviceID);
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