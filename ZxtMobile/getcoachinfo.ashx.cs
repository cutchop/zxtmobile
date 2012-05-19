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
            if (context.Request["ver"] != System.Configuration.ConfigurationManager.AppSettings["apkVersion"])
            {
                context.Response.Write("version_error");
            }
            else
            {
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
                catch (Exception ex)
                {
                    Logger.WriteLog("page:getcoachinfo.ashx;exception:" + ex.Message);
                }
                if (ds != null && ds.Tables[0] != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            sql = string.Format("select a.*,b.coach_time from gmit_app.coach_info a left join gmit_app.coach_card b on a.coach_id=b.coach_id where a.coach_school_id='{0}' and a.coach_id='{1}'", school, card);
                            ds = db.ExecuteReturnDataSet(sql);
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                DataRow dr = ds.Tables[0].Rows[0];
                                string id_card_no = dr["ID_CARD_NO"].ToString();
                                string certificate = dr["CERTIFICATE_ID"].ToString();
                                if (string.IsNullOrEmpty(id_card_no))
                                {
                                    id_card_no = "无";
                                }
                                if (string.IsNullOrEmpty(certificate))
                                {
                                    certificate = "无";
                                }
                                context.Response.Write(string.Format("s|{0}|{1}|{2}|{3}|{4}|{5}", card, dr["coach_time"], dr["code"], dr["name"], id_card_no, certificate));
                            }
                            else
                            {
                                context.Response.Write("f|教练身份验证失败");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteLog("page:getcoachinfo.ashx;exception:" + ex.Message);
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