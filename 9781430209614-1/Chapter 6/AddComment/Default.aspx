<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" Debug="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Add a coment to the current document</title>
    <link rel="stylesheet" type="text/css" href="/_layouts/1033/styles/core.css"/>
</head>
<body>
    <form id="form1" runat="server">
    <div style="margin: 10px 10px 10px 10px;">
    <table width="100%">
        <tr>
        <td width="50%">
        <table width="50%" style="font-size: smaller">
            <tr>
                <td style="width: 14px">
                    ItemId:</td>
                <td colspan="2">
                    <asp:Label ID="lblItemId" runat="server" Font-Bold="False"></asp:Label></td>
            </tr>
            <tr>
                <td style="width: 14px">
                    ItemUrl:</td>
                <td colspan="2">
                    &nbsp;<asp:HyperLink ID="hlItemUrl" runat="server" Target="_top">[hlItemUrl]</asp:HyperLink></td>
            </tr>
            <tr>
                <td style="width: 14px">
                    SiteUrl:</td>
                <td colspan="2">
                    &nbsp;<asp:HyperLink ID="hlSiteUrl" runat="server" Target="_top">[hlSiteUrl]</asp:HyperLink></td>
            </tr>
            <tr>
                <td style="width: 14px">
                    ListId:</td>
                <td colspan="2">
                    <asp:Label ID="lblListId" runat="server" Font-Bold="False"></asp:Label></td>
            </tr>
            <tr>
                <td style="width: 14px">
                    ListName:</td>
                <td>
                    <asp:Label ID="lblListName" runat="server" Font-Bold="False"></asp:Label></td>
            </tr>
            <tr>
                <td style="width: 14px">
                    ListUrl:</td>
                <td>
                    &nbsp;<asp:HyperLink ID="hlListUrl" runat="server" Target="_top">[hlListUrl]</asp:HyperLink></td>
            </tr>
            <tr>
                <td style="width: 14px">
                    &nbsp;</td>
                <td style="width: 100px">
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    Comments:<br />
                    <asp:TextBox ID="txtComments" runat="server" Rows="10" TextMode="MultiLine" Width="554px"></asp:TextBox><br />
                    &nbsp;</td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Button ID="cmdSave" runat="server" OnClick="cmdSave_Click" Text="Save Comments" />
                    <asp:Button ID="cmdCancel" runat="server" OnClick="cmdCancel_Click" Text="Cancel" /></td>
            </tr>
        </table>
        </td>
        <td width="50%" align="center" style="font-size: smaller" valign="top">
            <strong>Existing Comments:<br />
            </strong>
            <br />
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CellPadding="10">
                <Columns>
                    <asp:BoundField DataField="Comment" HeaderText="Comment" />
                    <asp:BoundField DataField="Author" HeaderText="Entered By" />
                    <asp:BoundField DataField="Created" HeaderText="Date Entered" />
                </Columns>
            </asp:GridView>
        </td>
        </tr>
        </table>
    </div>
    </form>
</body>
</html>
