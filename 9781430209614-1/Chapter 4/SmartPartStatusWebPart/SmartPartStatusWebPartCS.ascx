<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SmartPartStatusWebPartCS.ascx.cs" Inherits="SmartPartStatusWebPartCS" %>
<%@ Control AutoEventWireup="true" CodeFile="SmartPartStatusWebPartCS.ascx.cs" Inherits="SmartPartStatusWebPartCS"
    Language="C#" %>
<table id="Table2" border="0" cellpadding="6" cellspacing="1">
    <tr>
        <td>
            <asp:RadioButton ID="radRed" runat="server" AutoPostBack="True" GroupName="radWorkload"
                OnCheckedChanged="radRed_CheckedChanged" Text="I'm overwhelmed, full plate and then some, leave me alone" />
        </td>
        <td align="center" rowspan="3" valign="top">
            <asp:ImageButton ID="imgGreen" runat="server" ImageUrl="/_layouts/images/TRFFC10A.ICO"
                Visible="False" />
            <asp:ImageButton ID="imgYellow" runat="server" ImageUrl="/_layouts/images/TRFFC10B.ICO"
                Visible="False" />
            <asp:ImageButton ID="imgRed" runat="server" ImageUrl="/_layouts/images/TRFFC10C.ICO"
                Visible="False" />
        </td>
        <td align="center" rowspan="3" valign="middle">
        </td>
    </tr>
    <tr>
        <td>
            <asp:RadioButton ID="radYellow" runat="server" AutoPostBack="True" GroupName="radWorkload"
                OnCheckedChanged="radYellow_CheckedChanged" Text="I'm busy now, but nearing the end of a project and can take on new work (or will be able to soon)" />
        </td>
    </tr>
    <tr>
        <td>
            <asp:RadioButton ID="radGreen" runat="server" AutoPostBack="True" GroupName="radWorkload"
                OnCheckedChanged="radGreen_CheckedChanged" Text="I need work" />
        </td>
    </tr>
</table>
