using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZxtMobile
{
    public partial class scsfzzp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(FileUpload1.FileName))
            {
                if (FileUpload1.FileName.EndsWith(".bmp"))
                {
                    if (!string.IsNullOrEmpty(Request["id"]))
                    {
                        FileUpload1.SaveAs(System.Configuration.ConfigurationManager.AppSettings["photo"] + Request["id"] + ".bmp");                        
                    }
                    else
                    {
                        FileUpload1.SaveAs(System.Configuration.ConfigurationManager.AppSettings["photo"] + FileUpload1.FileName);
                    }
                    Panel1.Visible = false;
                    Response.Write("<script type=\"text/javascript\">alert('照片上传成功');window.close();</script>");
                }
                else
                {
                    Label1.Text = "请选择.bmp格式的照片";
                    Panel1.Visible = true;
                }
            }
            else
            {
                Label1.Text = "请选择照片";
                Panel1.Visible = true;
            }
        }
    }
}