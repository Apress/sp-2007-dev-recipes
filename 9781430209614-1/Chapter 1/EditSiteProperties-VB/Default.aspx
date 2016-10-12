<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

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
                    <td style="width: 100px">
                        Customer Id:</td>
                    <td style="width: 100px">
                        <asp:TextBox ID="txtCustomerId" runat="server"></asp:TextBox></td>
                    <td style="width: 100px">
                    </td>
                </tr>
                <tr>
                    <td style="width: 100px">
                        Bill Customer:</td>
                    <td style="width: 100px">
                        <asp:CheckBox ID="cbBillCustomer" runat="server" /></td>
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
                        <asp:Button ID="cmdSave" runat="server" OnClick="cmdSave_Click" Text="Save Changes" /></td>
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
                    <td colspan="3">
                        <asp:GridView ID="gvPropertyList" runat="server">
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </div>
    
    </div>
    </form>
</body>
</html>
