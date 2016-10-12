<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div>
            <table>
                <tr>
                    <td style="width: 163px">
                        Site collection path:</td>
                    <td style="width: 100px">
                        <asp:TextBox ID="txtSiteCollPath" runat="server">http://localhost</asp:TextBox></td>
                    <td style="width: 169px">
                    </td>
                </tr>
                <tr>
                    <td style="width: 163px">
                        Template:</td>
                    <td style="width: 100px">
                        <asp:DropDownList ID="ddlTemplate" runat="server">
                        </asp:DropDownList></td>
                    <td style="width: 169px">
                    </td>
                </tr>
                <tr>
                    <td style="width: 163px">
                        Web name:</td>
                    <td style="width: 100px">
                        <asp:TextBox ID="txtWebName" runat="server"></asp:TextBox></td>
                    <td style="width: 169px">
                    </td>
                </tr>
                <tr>
                </tr>
                <tr>
                </tr>
                <tr>
                </tr>
                <tr>
                    <td style="width: 163px">
                        Title:</td>
                    <td style="width: 100px">
                        <asp:TextBox ID="txtTitle" runat="server"></asp:TextBox></td>
                    <td style="width: 169px">
                    </td>
                </tr>
                <tr>
                    <td style="width: 163px">
                        Description:</td>
                    <td style="width: 100px">
                        <asp:TextBox ID="txtDescription" runat="server" Rows="5" TextMode="MultiLine" Width="350px"></asp:TextBox></td>
                    <td style="width: 169px">
                    </td>
                </tr>
                <tr>
                    <td style="width: 163px">
                    </td>
                    <td style="width: 100px">
                        &nbsp;</td>
                    <td style="width: 169px">
                    </td>
                </tr>
                <tr>
                    <td style="width: 163px">
                    </td>
                    <td style="width: 100px">
                        <asp:Button ID="cmdCreateWeb" runat="server" OnClick="cmdCreateWeb_Click1" Text="Create New Web Site" /></td>
                    <td style="width: 169px">
                    </td>
                </tr>
                <tr>
                    <td style="width: 163px">
                    </td>
                    <td style="width: 100px">
                        &nbsp;</td>
                    <td style="width: 169px">
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="3">
                        <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label></td>
                </tr>
            </table>
        </div>
    
    </div>
    </form>
</body>
</html>
