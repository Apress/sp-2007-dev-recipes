<%@ Page Language="VB" AutoEventWireup="false" CodeFile="SelectWebParts.aspx.vb" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div>
            Select the web-parts you want to add to this web site, then click the Finish button
            to save your changes and proceed to the site's home page:
            <br />
            <br />
            <table cellpadding="10">
                <tr>
                    <td colspan="2">
                        <asp:CheckBox ID="cbRemoveExisting" runat="server" Text="Remove Pre-Existing Web Parts" /></td>
                    <td colspan="1">
                    </td>
                    <td colspan="1">
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:CheckBox ID="cbCEWP" runat="server" Text="Content Editor Web Part" /></td>
                    <td colspan="1">
                    </td>
                    <td colspan="1">
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:CheckBox ID="cbPVWP" runat="server" AutoPostBack="True" OnCheckedChanged="cbPVWP_CheckedChanged"
                            Text="Page Viewer Web Part" /></td>
                    <td colspan="1">
                        Web address to use in Page Viewer:
                    </td>
                    <td colspan="1">
                        <asp:TextBox ID="txtPVUrl" runat="server" Enabled="False" Width="282px">http://www.yahoo.com</asp:TextBox></td>
                </tr>
            </table>
        </div>
        <br />
        <asp:Button ID="cmdFinish" runat="server" OnClick="Finish_Click" Text="Finish" />
    
    </div>
    </form>
</body>
</html>
