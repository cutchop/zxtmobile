using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Data;

namespace ZxtMobile
{
    /// <summary>
    /// 图片上传
    /// </summary>
    public class photoupload : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            if (context.Request.Files.Count > 0)
            {
                string deviceID = context.Request["deviceid"];
                string session = context.Request["session"];
                IDataBase db = DBConfig.GetDBObjcet();
                string sql = string.Format("select * from device_info where device_id='{0}' and device_session='{1}'", deviceID, session);
                DataSet ds = null;
                try
                {
                    ds = db.ExecuteReturnDataSet(sql);
                }
                catch (Exception ex)
                {
                    Logger.WriteLog("page:photoupload.ashx;exception:" + ex.Message + ";SQL:" + sql);
                }
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count == 0)
                {
                    context.Response.Write("f|设备非法");
                }
                else
                {
                    HttpPostedFile file = context.Request.Files[0];
                    try
                    {
                        string filename = context.Request["filename"];
                        string path = System.Configuration.ConfigurationManager.AppSettings["photosave"] + filename.Substring(0,8) + "/" + deviceID + "/";
                        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                        file.SaveAs(path + filename);
                        int speed =0;
                        try
                        {
                            speed = int.Parse(context.Request["speed"]);
                        }
                        catch { }
                        int senspeed = 0;
                        try
                        {
                            senspeed = int.Parse(context.Request["senspeed"]);
                        }
                        catch { }
                        string task_id = context.Request["guid"].Replace("null", "");
                        filename += ";";
                        sql = string.Format("update device_his_photo set file_name=file_name||'{0}'  where device_id='{1}' and logintime=to_date('{2}','yyyymmddhh24miss')", filename, deviceID, filename.Substring(0, 14));
                        if (db.ExecuteNonQuery(sql) == 0)
                        {
                            sql = string.Format("insert into device_his_photo(device_id, logintime, ph_type, file_name, file_url, task_id, speed, sen_speed) values('{0}',to_date('{1}','yyyymmddhh24miss'),0,'{2}','{3}','{4}',{5},{6})", deviceID, filename.Substring(0, 14), filename, path, task_id, speed, senspeed);
                            db.ExecuteNonQuery(sql);
                        }
                        context.Response.Write("s");
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog(ex.Message + "---sql:" + sql);
                    }
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