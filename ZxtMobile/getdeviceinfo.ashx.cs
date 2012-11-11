using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

namespace ZxtMobile
{
    /// <summary>
    /// getdeviceinfo 的摘要说明
    /// </summary>
    public class getdeviceinfo : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            if (string.IsNullOrEmpty(context.Request["imei"]))
            {
                context.Response.Write("failure|miss IMEI");
            }
            else
            {
                if (context.Request["ver"] != System.Configuration.ConfigurationManager.AppSettings["apkVersion"])
                {
                    context.Response.Write("failure|version_error");
                }
                else
                {
                    IDataBase db = DBConfig.GetDBObjcet();
                    string sql = string.Format(@"select a.device_id,a.device_name,b.orgid,b.orgname,a.jdq_type,a.camera,a.sensor_para,a.gps_sped_para,c.orgname as groupname,a.up_fg_image from device_info a 
                    left join gmit_base.aorg b on a.belong_companyid=b.orgid
                    left join gmit_base.aorg c on a.belong_groupid=c.orgid where a.device_imei='{0}'", context.Request["imei"]);
                    DataSet ds = null;
                    try
                    {
                        ds = db.ExecuteReturnDataSet(sql);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("page:getdeviceinfo.ashx;exception:" + ex.Message + ";SQL:" + sql);
                    }
                    if (ds != null && ds.Tables[0] != null)
                    {
                        DataTable dt = ds.Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            DataRow dr = dt.Rows[0];
                            string guid = Guid.NewGuid().ToString().Replace("-", "");
                            sql = string.Format("update device_info set device_session='{0}' where device_id='{1}'", guid, dr["device_id"]);
                            try
                            {
                                if (db.ExecuteNonQuery(sql) > 0)
                                {
                                    try
                                    {
                                        sql = string.Format("insert into device_work_log(device_id,log_type,device_imei) values('{0}','设备启动','{1}')", dr["device_id"], context.Request["imei"]);
                                        db.ExecuteNonQuery(sql);
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.WriteLog("page:getdeviceinfo.ashx;exception:" + ex.Message + ";SQL:" + sql);
                                        //context.Response.Write("failure|数据库异常"); 
                                    }
                                    string subject_para = "";
                                    try
                                    {
                                        sql = "select * from gmit_app.subject_para where subject=2 or subject=3 order by subject";
                                        DataTable spdt = db.ExecuteReturnDataSet(sql).Tables[0];
                                        subject_para += spdt.Rows[0]["sub_min_time"] + ",";
                                        subject_para += spdt.Rows[0]["sub_max_time"] + ",";
                                        subject_para += spdt.Rows[0]["day_max_time"] + ",";
                                        subject_para += spdt.Rows[1]["sub_min_time"] + ",";
                                        subject_para += spdt.Rows[1]["sub_max_time"] + ",";
                                        subject_para += spdt.Rows[1]["day_max_time"];
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.WriteLog("page:getdeviceinfo.ashx;exception:" + ex.Message + ";SQL:" + sql);
                                    }
                                    context.Response.Write(string.Format("success|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}", guid, dr["device_id"], dr["device_name"], dr["orgid"], dr["orgname"], dr["jdq_type"], dr["camera"], dr["sensor_para"], dr["gps_sped_para"], subject_para, dr["groupname"], dr["up_fg_image"]));
                                }
                                else
                                {
                                    context.Response.Write("failure|数据库异常");
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.WriteLog("page:getdeviceinfo.ashx;exception:" + ex.Message + ";SQL:" + sql);
                                context.Response.Write("failure|数据库异常");
                            }
                        }
                        else
                        {
                            context.Response.Write("failure|该设备没有注册(" + context.Request["imei"] + ")");
                        }
                    }
                    else
                    {
                        context.Response.Write("failure|数据库异常");
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