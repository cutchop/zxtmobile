<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="scsfzzp.aspx.cs" Inherits="ZxtMobile.scsfzzp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>上传照片</title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="font-size:14px;line-height:22px;margin:5px;">
    <asp:Panel ID="Panel1" Visible="false" runat="server">
    <div style="color:red;font-size:14px;line-height:22px;background-color:#ffff99;padding:3px;border:1px solid #ff9900;">
        *<asp:Label ID="Label1" runat="server" Text="请选择照片"></asp:Label>!
        </div>
    </asp:Panel>
    <div style="margin:10px 0 10px 0;font-weight:bold;">
    请选择照片文件，然后点击"上传"：</div>
    <div>
        <asp:FileUpload ID="FileUpload1" runat="server" />
        <asp:Button ID="Button1" runat="server" Text="上传" onclick="Button1_Click" /></div>
        <div style="font-size:12px;line-height:22px;margin-top:10px;color:Red;">*身份证照片暂时只支持.bmp文件</div>
        <div style="font-size:12px;line-height:22px;margin-top:5px;color:Red;">*照片尺寸：400px(宽) * 582px(高)</div>
    </div>
    </form>
</body>
</html>
