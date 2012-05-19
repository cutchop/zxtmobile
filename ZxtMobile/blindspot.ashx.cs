using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

namespace ZxtMobile
{
    /// <summary>
    /// GPS盲点
    /// </summary>
    public class blindspot : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string deviceID = context.Request["deviceid"];
            string session = context.Request["session"];
            string lng = context.Request["lng"];
            string lat = context.Request["lat"];
            string speed = context.Request["speed"];
            string gpstime = context.Request["gpstime"];

            IDataBase db = DBConfig.GetDBObjcet();
            string sql = string.Format("select * from device_info where device_id='{0}' and device_session='{1}'", deviceID, session);
            DataSet ds = null;
            try
            {
                ds = db.ExecuteReturnDataSet(sql);
            }
            catch (Exception ex)
            {
                Logger.WriteLog("page:blindspot.ashx;exception:" + ex.Message);
            }
            if (ds != null && ds.Tables[0] != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    try
                    {
                        sql = string.Format("insert into device_his_track(device_id,lon,lat,v_lon,v_lat,speed,logintime) values('{0}',{1},{2},{3},{4},{5},to_date('{6}','yyyy-mm-dd hh24:mi:ss'))", deviceID, lng, lat, lng, lat, speed, gpstime);
                        db.ExecuteNonQuery(sql);
                        context.Response.Write("s");
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("page:blindspot.ashx;exception:" + ex.Message);
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