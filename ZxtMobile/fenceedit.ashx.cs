using System;
using System.Collections.Generic;
using System.Web;

namespace ZxtMobile
{
    /// <summary>
    /// 修改栅栏
    /// </summary>
    public class fenceedit : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string type = context.Request["type"];
            if (!string.IsNullOrEmpty(type))
            {
                IDataBase db = DBConfig.GetDBObjcet();
                string sql = "";
                if (type == "1")
                {
                    //圆形
                    try
                    {
                        string id = context.Request["id"];
                        string name = context.Request["name"];
                        string data = context.Request["data"];
                        string lon = data.Split(';')[0].Split(',')[0];
                        string lat = data.Split(';')[0].Split(',')[1];
                        string rad = data.Split(';')[1];
                        sql = string.Format("update user_barrier set ba_name='{0}',ba_type='1',rad={1} where id={2}", name, rad, id);
                        db.ExecuteNonQuery(sql);
                        sql = string.Format("update user_barrier_detail set lon={0},lat={1} where id={2}", lon, lat, id);
                        db.ExecuteNonQuery(sql);
                        context.Response.Write("s");
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("page:fenceeidt.ashx;exception:" + ex.Message + ";SQL:" + sql);
                        context.Response.Write("f");
                    }
                }
                else if (type == "2")
                {
                    //矩形
                    try
                    {
                        string id = context.Request["id"];
                        string name = context.Request["name"];
                        string data = context.Request["data"];
                        string[] arr = data.Split(',');
                        sql = string.Format("update user_barrier set ba_name='{0}',ba_type='2' where id={1}", name, id);
                        db.ExecuteNonQuery(sql);
                        sql = "delete from user_barrier_detail where id="+id;
                        db.ExecuteNonQuery(sql);
                        for (int i = 0; i < arr.Length; i++)
                        {
                            sql = string.Format("insert into user_barrier_detail(id,line,lon,lat) values({0},{1},{2},{3})", id, i / 2 + 1, arr[i], arr[++i]);
                            db.ExecuteNonQuery(sql);
                        }
                        context.Response.Write("s");
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("page:fenceedit.ashx;exception:" + ex.Message + ";SQL:" + sql);
                        context.Response.Write("f");
                    }
                }
                else
                {
                    //多边形
                    try
                    {
                        string id = context.Request["id"];
                        string name = context.Request["name"];
                        string data = context.Request["data"];
                        string[] arr = data.Split(',');
                        sql = string.Format("update user_barrier set ba_name='{0}',ba_type='3' where id={1}", name, id);
                        db.ExecuteNonQuery(sql);
                        sql = "delete from user_barrier_detail where id=" + id;
                        db.ExecuteNonQuery(sql);
                        for (int i = 0; i < arr.Length; i++)
                        {
                            sql = string.Format("insert into user_barrier_detail(id,line,lon,lat) values({0},{1},{2},{3})", id, i / 2 + 1, arr[i], arr[++i]);
                            db.ExecuteNonQuery(sql);
                        }
                        context.Response.Write("s");
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("page:fenceedit.ashx;exception:" + ex.Message + ";SQL:" + sql);
                        context.Response.Write("f");
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