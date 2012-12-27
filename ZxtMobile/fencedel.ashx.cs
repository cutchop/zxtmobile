using System;
using System.Collections.Generic;
using System.Web;

namespace ZxtMobile
{
    /// <summary>
    /// 删除栅栏
    /// </summary>
    public class fencedel : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string id = context.Request["id"];
            if (!string.IsNullOrEmpty(id))
            {
                IDataBase db = DBConfig.GetDBObjcet();
                string sql = "delete from user_barrier where id=" + id;
                try
                {
                    db.ExecuteNonQuery(sql);
                    sql = "delete from user_barrier_detail where id=" + id;
                    db.ExecuteNonQuery(sql);
                    context.Response.Write("s");
                }
                catch (Exception ex)
                {
                    Logger.WriteLog("page:fencedel.ashx;exception:" + ex.Message + ";SQL:" + sql);
                    context.Response.Write("f");
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