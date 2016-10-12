<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table width="80%">
            <tr>
                <td colspan="3">
                    <strong style="color: blue">I. First enter site collection url and web site name, then click "Lookup" button to
                    display rest of form<br />
                    </strong>
                </td>
            </tr>
            <tr>
                <td>
                    Site collection url:</td>
                <td colspan="2">
                    <asp:TextBox ID="txtSiteUrl" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtSiteUrl"
                        ErrorMessage="Site collection url required"></asp:RequiredFieldValidator></td>
            </tr>
            <tr>
                <td>
                    Web site name:</td>
                <td colspan="2">
                    <asp:TextBox ID="txtWebName" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td align="center" colspan="3">
                    <asp:Button ID="cmdLookupListTemplates" runat="server" OnClick="cmdLookupListTemplates_Click"
                        Text="Lookup up List Templates" /></td>
            </tr>
            <tr>
                <td colspan="3">
                    &nbsp;</td>
            </tr>
            </table>
        <asp:Panel ID="Panel1" runat="server" Height="50px" Visible="False" Width="100%">
            <table width="80%" id="TABLE1" language="javascript" onclick="return TABLE1_onclick()">
            <tr>
                <td colspan="3" style="color: blue">
                    <strong>II.
                    Then fill in rest of data and click "Create" button<br />
                    </strong>
                </td>
            </tr>
            <tr>
                <td style="width: 133px">
                    Name of new list:</td>
                <td style="width: 312px">
                    <asp:TextBox ID="txtListName" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtListName"
                        ErrorMessage="List name required"></asp:RequiredFieldValidator></td>
                <td >
                </td>
            </tr>
            <tr>
                <td style="width: 133px">
                    Type of list:</td>
                <td style="width: 312px">
                    <asp:DropDownList ID="ddlListType" runat="server">
                    </asp:DropDownList></td>
                <td>
                </td>
            </tr>
            <tr>
                <td style="width: 133px">
                    &nbsp;</td>
                <td style="width: 312px">
                </td>
                <td >
                </td>
            </tr>
            <tr>
                <td style="width: 133px">
                    Custom column #1:</td>
                <td style="width: 312px">
                    Name:
                    <asp:TextBox ID="txtUDFName1" runat="server"></asp:TextBox></td>
                <td >
                    Type:
                    <asp:DropDownList ID="ddlUDFType1" runat="server">
                    </asp:DropDownList></td>
            </tr>
            <tr>
                <td style="width: 133px">
                    Custom column #2:</td>
                <td style="width: 312px">
                    Name:
                    <asp:TextBox ID="txtUDFName2" runat="server"></asp:TextBox></td>
                <td >
                    Type:
                    <asp:DropDownList ID="ddlUDFType2" runat="server">
                    </asp:DropDownList></td>
            </tr>
            <tr>
                <td style="width: 133px">
                    Custom column #3:</td>
                <td style="width: 312px">
                    Name:
                    <asp:TextBox ID="txtUDFName3" runat="server"></asp:TextBox></td>
                <td >
                    Type:
                    <asp:DropDownList ID="ddlUDFType3" runat="server">
                    </asp:DropDownList></td>
            </tr>
            <tr>
                <td >
                    &nbsp;
                </td>
                <td style="width: 312px" >
                </td>
                <td >
                </td>
            </tr>
            <tr>
                <td align="center" colspan="3">
                    <asp:CheckBox ID="cbOpenList" runat="server" Text="Open new list after it's been created" /></td>
            </tr>
            <tr>
                <td align="center" colspan="3">
                    <asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red" Visible="False"></asp:Label></td>
            </tr>
            <tr>
                <td align="center" colspan="3">
                    <asp:Button ID="cmdCreateList" runat="server" Text="Create New List" OnClick="cmdCreateList_Click" /></td>
            </tr>
        </table>
        </asp:Panel>
        <br />
    
    </div>
    </form>
</body>
</html>
