<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="CustomizingSearch.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    <link type="text/css" href="StyleSheet.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox ID="txtSearch" runat="server" Width="287px"></asp:TextBox>
        <asp:Button ID="cmdSearch" runat="server" OnClick="cmdSearch_Click" Text="Search" /><br />
        <br />
        <asp:GridView ID="gridSearch" runat="server" AutoGenerateColumns="False" CellPadding="4"
            Font-Size="Smaller" ForeColor="#333333" GridLines="None">
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <Columns>
                <asp:HyperLinkField DataNavigateUrlFields="Path" DataTextField="Title" HeaderText="Title"
                    Text="Title" />
                <asp:BoundField DataField="Rank" HeaderText="Rank" />
                <asp:BoundField />
                <asp:BoundField DataField="Write" HeaderText="Edit Date" />
                <asp:BoundField DataField="HitHighlightedSummary" HeaderText="Summary" HtmlEncode="False" HtmlEncodeFormatString="False" />
                    <asp:BoundField DataField="ContentSource" HeaderText="ContentSource" />
            </Columns>
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#999999" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        </asp:GridView>
        <br />
        <asp:Label ID="lblMsg" runat="server" Font-Bold="true"></asp:Label></div>
    </form>
</body>
</html>
