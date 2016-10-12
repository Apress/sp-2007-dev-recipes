<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table width="100%">
            <tr>
                <td>
                    Site collection Url:</td>
                <td style="width: 100px">
                    <asp:TextBox ID="txtSiteUrl" runat="server"></asp:TextBox></td>
                <td style="width: 100px">
                </td>
            </tr>
            <tr>
                <td>
                    Web site name:</td>
                <td style="width: 100px">
                    <asp:TextBox ID="txtWebName" runat="server"></asp:TextBox></td>
                <td style="width: 100px">
                </td>
            </tr>
            <tr>
                <td>
                    New role definition name:</td>
                <td style="width: 100px">
                    <asp:TextBox ID="txtRoleName" runat="server"></asp:TextBox></td>
                <td style="width: 100px">
                </td>
            </tr>
            <tr>
                <td>
                    New role definition</td>
                <td style="width: 100px">
                    <asp:TextBox ID="txtRoleDefinition" runat="server" Rows="5" TextMode="MultiLine" Width="300px"></asp:TextBox></td>
                <td style="width: 100px">
                </td>
            </tr>
            <tr>
                <td>
                    Copy permissions from which role:</td>
                <td style="width: 100px">
                    <asp:RadioButtonList ID="rblTemplateRole" runat="server">
                        <asp:ListItem>Full Control</asp:ListItem>
                        <asp:ListItem>Contribute</asp:ListItem>
                        <asp:ListItem Selected="True">Read</asp:ListItem>
                    </asp:RadioButtonList></td>
                <td style="width: 100px">
                </td>
            </tr>
            <tr>
                <td style="width: 100px">
                </td>
                <td style="width: 100px">
                    <asp:Button ID="cmdAddRole" runat="server" OnClick="cmdAddRole_Click" Text="Add Role" /></td>
                <td style="width: 100px">
                </td>
            </tr>
            <tr>
                <td style="width: 100px">
                </td>
                <td style="width: 100px">
                    &nbsp;</td>
                <td style="width: 100px">
                </td>
            </tr>
            <tr>
                <td style="width: 100px">
                </td>
                <td style="width: 100px">
                    <asp:Label ID="lblReturnMsg" runat="server" ForeColor="Red"></asp:Label></td>
                <td style="width: 100px">
                </td>
            </tr>
        </table>
    
    </div>
    </form>
</body>
</html>
