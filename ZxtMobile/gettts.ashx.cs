using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Text;

namespace ZxtMobile
{
    /// <summary>
    /// 获取TTS
    /// </summary>
    public class gettts : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string deviceID = context.Request["deviceid"];
            string session = context.Request["session"];
            string school = context.Request["school"];
            string ver = context.Request["ver"];

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
                        sql = string.Format("select item_context from zxt_app.project3_itme where item_id=0 and item_context='{0}'", ver);
                        ds = db.ExecuteReturnDataSet(sql);
                        if (ds.Tables[0].Rows.Count == 0)
                        {
                            sql = string.Format("select * from zxt_app.project3_itme order by item_id");
                            ds = db.ExecuteReturnDataSet(sql);
                            DataTable table = ds.Tables[0];
                            StringBuilder sb = new StringBuilder();
                            sb.Append("s|");
                            for (int i = 0; i < table.Rows.Count; i++)
                            {
                                sb.AppendFormat("{0}#{1}#{2}|", table.Rows[i]["ITEM_ID"], table.Rows[i]["ITEM_NAME"], table.Rows[i]["ITEM_CONTEXT"]);
                            }
                            context.Response.Write(sb.ToString());
                        }
                        else
                        {
                            context.Response.Write("s|");
                        }
                    }
                    catch
                    {
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