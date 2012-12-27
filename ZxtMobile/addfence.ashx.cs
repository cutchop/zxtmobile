using System;
using System.Collections.Generic;
using System.Web;

namespace ZxtMobile
{
    /// <summary>
    /// 添加栅栏
    /// </summary>
    public class addfence : IHttpHandler
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
                        sql = "select nvl(max(id),0)+1 as newid from user_barrier";
                        string id = db.ExecuteReturnDataSet(sql).Tables[0].Rows[0][0].ToString();
                        string name = context.Request["name"];
                        string data = context.Request["data"];
                        string lon = data.Split(';')[0].Split(',')[0];
                        string lat = data.Split(';')[0].Split(',')[1];
                        string rad = data.Split(';')[1];
                        sql = string.Format("insert into user_barrier(id,ba_name,ba_type,showflag,rad) values({0},'{1}','1','1',{2})", id, name, rad);
                        db.ExecuteNonQuery(sql);
                        sql = string.Format("insert into user_barrier_detail(id,line,lon,lat) values({0},1,{1},{2})",id,lon,lat);
                        db.ExecuteNonQuery(sql);
                        context.Response.Write("s:"+id);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("page:addfence.ashx;exception:" + ex.Message + ";SQL:" + sql);
                        context.Response.Write("f");
                    }
                }
                else if (type == "2")
                {
                    //矩形
                    try
                    {
                        sql = "select nvl(max(id),0)+1 as newid from user_barrier";
                        string id = db.ExecuteReturnDataSet(sql).Tables[0].Rows[0][0].ToString();
                        string name = context.Request["name"];
                        string data = context.Request["data"];
                        string[] arr = data.Split(',');
                        sql = string.Format("insert into user_barrier(id,ba_name,ba_type,showflag,rad) values({0},'{1}','2','1',0)", id, name);
                        db.ExecuteNonQuery(sql);
                        for (int i = 0; i < arr.Length; i++)
                        {
                            sql = string.Format("insert into user_barrier_detail(id,line,lon,lat) values({0},{1},{2},{3})", id, i / 2 + 1, arr[i], arr[++i]);
                            db.ExecuteNonQuery(sql);
                        }
                        context.Response.Write("s:" + id);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("page:addfence.ashx;exception:" + ex.Message + ";SQL:" + sql);
                        context.Response.Write("f");
                    }
                }
                else
                {
                    //多边形
                    try
                    {
                        sql = "select nvl(max(id),0)+1 as newid from user_barrier";
                        string id = db.ExecuteReturnDataSet(sql).Tables[0].Rows[0][0].ToString();
                        string name = context.Request["name"];
                        string data = context.Request["data"];
                        string[] arr = data.Split(',');
                        sql = string.Format("insert into user_barrier(id,ba_name,ba_type,showflag,rad) values({0},'{1}','3','1',0)", id, name);
                        db.ExecuteNonQuery(sql);
                        for (int i = 0; i < arr.Length; i++)
                        {
                            sql = string.Format("insert into user_barrier_detail(id,line,lon,lat) values({0},{1},{2},{3})", id, i / 2 + 1, arr[i], arr[++i]);
                            db.ExecuteNonQuery(sql);
                        }
                        context.Response.Write("s:" + id);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("page:addfence.ashx;exception:" + ex.Message + ";SQL:" + sql);
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