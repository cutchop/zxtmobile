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
                    string sql = string.Format(@"select a.device_id,a.device_name,b.orgid,b.orgname from device_info a 
                                    left join gmit_base.aorg b on a.belong_companyid=b.orgid where a.device_imei='{0}'", context.Request["imei"]);
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
                                    context.Response.Write(string.Format("success|{0}|{1}|{2}|{3}|{4}", guid, dr["device_id"], dr["device_name"], dr["orgid"], dr["orgname"]));
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