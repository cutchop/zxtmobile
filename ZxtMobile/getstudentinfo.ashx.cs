using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

namespace ZxtMobile
{
    /// <summary>
    /// 获取学员信息
    /// </summary>
    public class getstudentinfo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
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
            string sql = string.Format("select check_finger,finger_get_flag from gmit_app.student_info where self_18='{0}'", context.Request["card"]);
            DataSet ds = null;
            IDataBase db = DBConfig.GetDBObjcet();
            try
            {
                ds = db.ExecuteReturnDataSet(sql);
            }
            catch (Exception ex)
            {
                Logger.WriteLog("page:getstudentinfo.ashx;exception:" + ex.Message + ";SQL:" + sql);
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
            string cardbalance = context.Request["balance"];
            string school = context.Request["school"];
            if (string.IsNullOrEmpty(cardbalance))
            {
                cardbalance = "0";
            }

            IDataBase db = DBConfig.GetDBObjcet();
            string sql = string.Format("select * from device_info where device_id='{0}' and device_session='{1}'", deviceID, session);
            DataSet ds = null;
            try
            {
                ds = db.ExecuteReturnDataSet(sql);
            }
            catch (Exception ex)
            {
                Logger.WriteLog("page:getstudentinfo.ashx;exception:" + ex.Message + ";SQL:" + sql);
            }
            if (ds != null && ds.Tables[0] != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0]["use_status"].ToString().Trim() != "正常")
                    {
                        context.Response.Write("f|车辆不合格");
                        return;
                    }
                    if (!string.IsNullOrEmpty(context.Request["coach"]))
                    {
                        try
                        {
                            sql = string.Format("select coach_status from gmit_app.coach_info where coach_id='{0}'", context.Request["coach"]);
                            ds = db.ExecuteReturnDataSet(sql);
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                if (ds.Tables[0].Rows[0]["coach_status"].ToString().Trim() != "正常")
                                {
                                    context.Response.Write("f|教练不合格");
                                    return;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteLog("page:getstudentinfo.ashx;exception:" + ex.Message + ";SQL:" + sql);
                        }
                    }
                    try
                    {
                        sql = string.Format("select a.*,b.yue_num as stu_shichang,gmit_app.get_train_status(a.self_18) as sub from gmit_app.student_info a left join gmit_app.stu_yue b on a.stuinfo_id=b.stuinfo_id where a.school_id='{0}' and a.self_18='{1}'", school, card);
                        ds = db.ExecuteReturnDataSet(sql);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = ds.Tables[0].Rows[0];
                            string id_card_no = dr["ID_CARD_NO"].ToString();
                            string driver_type = dr["DRIVER_TYPE"].ToString();
                            if (string.IsNullOrEmpty(id_card_no))
                            {
                                id_card_no = "无";
                            }
                            if (string.IsNullOrEmpty(driver_type))
                            {
                                driver_type = "无";
                            }
                            try
                            {
                                // 记录卡上余额
                                sql = string.Format("insert into gmit_app.card_balance_log(school_id, card, card_balance, server_balance, device_id) values('{0}','{1}',{2},{3},'{4}')", school, card, cardbalance, dr["stu_shichang"], deviceID);
                                db.ExecuteNonQuery(sql);
                                // 更新充值到账时间
                                sql = string.Format("update gmit_app.card_opr set remote_run_time=sysdate,device_id='{0}',add_type=9 where use_type='学员卡' and card_id='{1}' and remote_run_time is null", deviceID, card);
                                db.ExecuteNonQuery(sql);
                            }
                            catch (Exception ex)
                            {
                                Logger.WriteLog("page:getstudentinfo.ashx;exception:" + ex.Message + ";SQL:" + sql);
                            }
                            string totaltime, totalmi, daytime, daymi;
                            totaltime = totalmi = daytime = daymi = "0";
                            try
                            {
                                // 累计学时和里程
                                sql = string.Format(@"select nvl(sum(use_datetime),0) as totaltime,nvl(sum(use_mi),0) as totalmi from gmit_app.jx_use_data where school_id='{0}' and card_type='02' and card_id in 
                                    (select stu_id from gmit_app.student_card where stu_code=(select stuinfo_id from gmit_app.student_info where school_id='{0}' and self_18='{1}'))",school,card);
                                DataTable table = db.ExecuteReturnDataSet(sql).Tables[0];
                                totaltime = table.Rows[0]["totaltime"].ToString();
                                totalmi = table.Rows[0]["totalmi"].ToString();
                                // 当日学时和里程
                                sql = string.Format(@"select nvl(sum(use_datetime),0) as daytime,nvl(sum(use_mi),0) as daymi from gmit_app.jx_use_data where school_id='{0}' and card_type='02' and card_id in 
                                    (select stu_id from gmit_app.student_card where stu_code=(select stuinfo_id from gmit_app.student_info where school_id='{0}' and self_18='{1}'))
                                    and to_char(start_time,'yyyymmdd')=to_char(sysdate,'yyyymmdd')",school, card);
                                table = db.ExecuteReturnDataSet(sql).Tables[0];
                                daytime = table.Rows[0]["daytime"].ToString();
                                daymi = table.Rows[0]["daymi"].ToString();
                            }
                            catch (Exception ex)
                            {
                                Logger.WriteLog("page:getstudentinfo.ashx;exception:" + ex.Message + ";SQL:" + sql);
                            }
                            context.Response.Write(string.Format("s|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9},{10},{11},{12}|{13}", card, dr["stu_shichang"], dr["stuinfo_id"], dr["stu_name"], id_card_no, driver_type, daytime, dr["sub"], dr["is_charging"], totaltime, totalmi, daytime, daymi, dr["exam_status"]));
                        }
                        else
                        {
                            context.Response.Write("f|学员身份验证失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("page:getstudentinfo.ashx;exception:" + ex.Message + ";SQL:" + sql);
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