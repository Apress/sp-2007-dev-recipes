<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Upload File</title>
</head>
<body>
    <form id="form1" runat="server">
   <div>
        <table width="100%">
            <tr>
                <td width="200">
                    Site collection url:</td>
                <td style="width: 100px">
                    <asp:TextBox ID="txtSiteUrl" runat="server"></asp:TextBox></td>
                <td style="width: 100px">
                </td>
            </tr>
            <tr>
                <td width="200">
                    Web site name:</td>
                <td style="width: 100px">
                    <asp:TextBox ID="txtWebName" runat="server"></asp:TextBox></td>
                <td style="width: 100px">
                </td>
            </tr>
            <tr>
                <td width="200">
                    Document library:</td>
                <td style="width: 100px">
                    <asp:TextBox ID="txtDocLibName" runat="server"></asp:TextBox></td>
                <td style="width: 100px">
                </td>
            </tr>
            <tr>
                <td width="200">
                    File to upload:
                </td>
                <td style="width: 100px">
                    <asp:FileUpload ID="FileUpload1" runat="server" /></td>
                <td style="width: 100px">
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    &nbsp;</td>
                <td style="width: 100px">
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Button ID="cmdUploadFile" runat="server" OnClick="cmdUploadFile_Click" Text="Upload File" /></td>
                <td style="width: 100px">
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    &nbsp;</td>
                <td style="width: 100px">
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red"></asp:Label></td>
                <td style="width: 100px">
                </td>
            </tr>
        </table>
    
    </div>
    </form>
</body>
</html>
