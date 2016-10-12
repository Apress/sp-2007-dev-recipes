<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
        <table>
            <tr>
                <td style="width: 175px">
                    Command:</td>
                <td style="width: 100px">
                    <asp:DropDownList ID="ddlCommand" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCommand_SelectedIndexChanged">
                    </asp:DropDownList></td>
                <td style="width: 100px">
                    &nbsp;</td>
                <td align="left" colspan="2" rowspan="7" valign="top">
                    <strong>List of Employees</strong><br />
                    <br />
        <asp:GridView ID="GridView1" runat="server">
        </asp:GridView>
                </td>
            </tr>
            <tr>
                <td style="width: 175px">
                    ID:</td>
                <td style="width: 100px">
                    <asp:DropDownList ID="ddlID" runat="server">
                    </asp:DropDownList></td>
                <td style="width: 100px">
                </td>
            </tr>
            <tr>
                <td style="width: 175px">
                    Employee Name:</td>
                <td style="width: 100px">
                    <asp:TextBox ID="txtEmpName" runat="server"></asp:TextBox></td>
                <td style="width: 100px">
                </td>
            </tr>
            <tr>
                <td style="width: 175px">
                    Job Title:</td>
                <td style="width: 100px">
                    <asp:TextBox ID="txtJobTitle" runat="server"></asp:TextBox></td>
                <td style="width: 100px">
                </td>
            </tr>
            <tr>
                <td style="width: 175px; height: 21px">
                    Hire Date:</td>
                <td style="width: 100px; height: 21px">
                    <asp:TextBox ID="txtHireDate" runat="server"></asp:TextBox></td>
                <td style="width: 100px; height: 21px">
                </td>
            </tr>
            <tr>
                <td style="width: 175px; height: 21px">
                    &nbsp;
                </td>
                <td style="width: 100px; height: 21px">
                </td>
                <td style="width: 100px; height: 21px">
                </td>
            </tr>
            <tr>
                <td style="width: 175px; height: 21px">
                </td>
                <td style="width: 100px; height: 21px">
                    <asp:Button ID="Button1" runat="server" Text="Go..." OnClick="Button1_Click" /></td>
                <td style="width: 100px; height: 21px">
                </td>
            </tr>
            <tr>
                <td colspan="5" style="height: 21px">
                    &nbsp;</td>
            </tr>
            <tr>
                <td colspan="5" style="height: 21px">
                    <asp:Label ID="lblBatchXML" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td colspan="5" style="height: 21px">
                    &nbsp;</td>
            </tr>
            <tr>
                <td colspan="5" style="height: 21px">
                    <asp:Label ID="lblReturnXML" runat="server"></asp:Label></td>
            </tr>
        </table>
        <br />
        &nbsp;
    </form>
</body>
</html>
