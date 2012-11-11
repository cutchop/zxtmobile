using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;

namespace ZxtMobile
{
    public partial class fence : System.Web.UI.Page
    {
        protected string VarInit = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("var fences = new Array();");
                IDataBase db = DBConfig.GetDBObjcet();
                DataTable dt, dt2;
                string sql = "select * from user_barrier";
                try
                {
                    dt = db.ExecuteReturnDataSet(sql).Tables[0];
                    for (int i = 0; i < dt.Rows.Count;i++ )
                    {
                        sb.AppendFormat("fences[{0}] = new Array();",i);
                        sb.AppendFormat("fences[{0}].id = \"{1}\";", i,dt.Rows[i]["id"]);
                        sb.AppendFormat("fences[{0}].type = \"{1}\";",i, dt.Rows[i]["ba_type"]);
                        if (dt.Rows[i]["ba_type"].ToString() != "1")
                        {
                            sb.AppendFormat("fences[{0}].arr = new Array();", i);
                        }
                        sql = "select * from user_barrier_detail where id=" + dt.Rows[i]["id"];
                        dt2 = db.ExecuteReturnDataSet(sql).Tables[0];
                        for (int j= 0; j < dt2.Rows.Count; j++)
                        {
                            if (dt.Rows[i]["ba_type"].ToString() == "1")
                            {
                                sb.AppendFormat("fences[{0}].center=new MMap.LngLat(\"{1}\", \"{2}\");", i, dt2.Rows[j]["lon"], dt2.Rows[j]["lat"]);
                                sb.AppendFormat("fences[{0}].radius={1};",i,dt.Rows[i]["rad"]);
                            }
                            else
                            {
                                sb.AppendFormat("fences[{0}].arr.push(new MMap.LngLat(\"{1}\", \"{2}\"));", i, dt2.Rows[j]["lon"], dt2.Rows[j]["lat"]);
                            }
                        }
                    }
                    VarInit = sb.ToString();
                }
                catch (Exception ex)
                {
                    Logger.WriteLog("page:fence.aspx.cs;exception:" + ex.Message + ";SQL:" + sql);
                }
            }
        }
    }
}