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
            if (context.Request["action"] == "checkfinger")
            {
                if (string.IsNullOrEmpty(context.Request["card"]))
                {
                    context.Response.Write("1");
                }
                else
                {
                    checkfinger(context);
                }
            }
            else
            {
                context.Response.ContentType = "text/plain";
                if (context.Request["ver"] != System.Configuration.ConfigurationManager.AppSettings["apkVersion"])
                {
                    context.Response.Write("version_error");
                }
                else
                {
                    getinfo(context);
                }
            }
        }


        private void checkfinger(HttpContext context)
        {
            string sql = string.Format("select check_finger,finger_get_flag from gmit_app.coach_info where coach_id='{0}'", context.Request["card"]);
            DataSet ds = null;
            IDataBase db = DBConfig.GetDBObjcet();
            try
            {
                ds = db.ExecuteReturnDataSet(sql);
            }
            catch (Exception ex)
            {
                Logger.WriteLog("page:getcoachtinfo.ashx;exception:" + ex.Message + ";SQL:" + sql);
            }
            if (ds != null && ds.Tables[0] != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0]["check_finger"].ToString() == "1")
                    {
                        if (ds.Tables[0].Rows[0]["finger_get_flag"].ToString() == "0")
                        {
                            context.Response.Write("2");//需要下载并验证指纹
                        }
                        else
                        {
                            context.Response.Write("1");
                        }
                    }
                    else
                    {
                        context.Response.Write("0");
                    }
                }
                else
                {
                    context.Response.Write("1");
                }
            }
            else
            {
                context.Response.Write("1");
            }
        }


        private void getinfo(HttpContext context)
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
                Logger.WriteLog("page:getcoachinfo.ashx;exception:" + ex.Message + ";SQL:" + sql);
            }
            if (ds != null && ds.Tables[0] != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    try
                    {
                        sql = string.Format("select a.*,b.yue as coach_time from gmit_app.coach_info a left join gmit_app.coach_yue b on a.code=b.code where a.coach_school_id='{0}' and a.coach_id='{1}'", school, card);
                        ds = db.ExecuteReturnDataSet(sql);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = ds.Tables[0].Rows[0];
                            if (dr["coach_status"].ToString().Trim() != "正常")
                            {
                                context.Response.Write("f|教练不合格");
                                return;
                            }
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
                            string monthtime, daytime;
                            monthtime = daytime = "0";
                            try
                            {
                                // 当月学时
                                sql = string.Format(@"select nvl(sum(use_datetime),0) as ttime 
                                    from gmit_app.jx_use_data where school_id='{0}' and card_type='02' and coach_id='{1}'", school, card);
                                DataTable table = db.ExecuteReturnDataSet(sql + " and to_char(start_time,'yyyymm')=to_char(sysdate,'yyyymm')").Tables[0];
                                monthtime = table.Rows[0]["ttime"].ToString();
                                // 当日学时
                                sql += " and to_char(start_time,'yyyymmdd')=to_char(sysdate,'yyyymmdd')";
                                table = db.ExecuteReturnDataSet(sql).Tables[0];
                                daytime = table.Rows[0]["ttime"].ToString();
                            }
                            catch (Exception ex)
                            {
                                Logger.WriteLog("page:getstudentinfo.ashx;exception:" + ex.Message + ";SQL:" + sql);
                            }
                            context.Response.Write(string.Format("s|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8},{9}", card, dr["coach_time"], dr["code"], dr["name"], id_card_no, certificate, dr["is_charging"], dr["coach_level"], monthtime, daytime));
                        }
                        else
                        {
                            context.Response.Write("f|教练身份验证失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("page:getcoachinfo.ashx;exception:" + ex.Message + ";SQL:" + sql);
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

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}