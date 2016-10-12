Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Drawing

Namespace ZoneTabWebPartVB
    Public Class ZoneTabWebPart
        Inherits WebPart
        ' Local variables 
        Private _tabData As String = ""
        Private _debug As Boolean = False
        Private _selectedTab As String
        Private _tabWidth As Integer = 100
        Private _tabBackgroundColorSelected As String = "white"
        Private _tabBackgroundColorDeselected As String = "whitesmoke"

        <Personalizable()> _
        <WebBrowsable()> _
        <WebDisplayName("Flag indicating whether debug info should be displayed")> _
        Public Property Debug() As Boolean
            Get
                Return _debug
            End Get
            Set(ByVal value As Boolean)
                _debug = value
            End Set
        End Property

        ' String containing semi-colon delimited list 
        ' of tab names. Tab names are preceeded by "*". 
        ' 
        ' Example: *Tab 1;webpart1;webpart2;*Tab 2;webpart3 
        ' 
        <Personalizable()> _
        <WebBrowsable()> _
        <WebDisplayName("A delimited list of tab names and associated web parts")> _
        Public Property TabData() As String
            Get
                Return _tabData
            End Get
            Set(ByVal value As String)
                _tabData = value
            End Set
        End Property

        <Personalizable()> _
        <WebBrowsable()> _
        <WebDescription("Color of selected tab")> _
        Public Property SelectedColor() As String
            Get
                Return _tabBackgroundColorSelected
            End Get
            Set(ByVal value As String)
                _tabBackgroundColorSelected = value
            End Set
        End Property

        <Personalizable()> _
        <WebBrowsable()> _
        <WebDescription("Color of un-selected tab")> _
        Public Property DeSelectedColor() As String
            Get
                Return _tabBackgroundColorDeselected
            End Get
            Set(ByVal value As String)
                _tabBackgroundColorDeselected = value
            End Set
        End Property

        <Personalizable()> _
        <WebBrowsable()> _
        <WebDisplayName("Width in pixels for each tab")> _
        Public Property TabWidth() As Integer
            Get
                Return _tabWidth
            End Get
            Set(ByVal value As Integer)
                _tabWidth = value
            End Set
        End Property

        ' Add tab-links to page 
        Protected Overloads Overrides Sub CreateChildControls()
            MyBase.CreateChildControls()
            Try

                Dim arrTabs As String() = _tabData.Split(";"c)

                ' Build list of tabs in the form 
                ' of an HTML <TABLE> with <A> tags 
                ' for each tab 

                ' Step 1: Define <TABLE> and <TR> HTML elements 
                Dim tbl As New Table()
                tbl.CellPadding = 0
                tbl.CellSpacing = 0

                Dim tr As New TableRow()
                tr.HorizontalAlign = HorizontalAlign.Left

                ' Step 2: Loop through list of tabs, adding 
                ' <TD> and <A> HTML elements for each 
                Dim tc As TableCell
                Dim tab As LinkButton
                Dim tabCount As Integer = 0
                For i As Integer = 0 To arrTabs.Length - 1
                    If arrTabs(i).IndexOf("*") = 0 Then
                        ' Add a blank separator cell 
                        tc = New TableCell()
                        tc.Text = "&nbsp;"
                        tc.Width = System.Web.UI.WebControls.Unit.Percentage(1)
                        tc.Style("border-bottom") = "black 1px solid"
                        tr.Cells.Add(tc)

                        ' Step 4: Create a <TD> HTML element to hold the tab 
                        tc = New TableCell()
                        tc.ID = "tc_" + arrTabs(i).Substring(1).Replace(" ", "_")
                        tc.Width = System.Web.UI.WebControls.Unit.Pixel(_tabWidth)

                        ' Step 5: Create an <A> HTML element to represent 
                        ' the tab. Discard first character, which 
                        ' was a "*" 
                        tab = New LinkButton()
                        tab.ID = "tab_" + arrTabs(i).Substring(1).Replace(" ", "_")
                        tab.Text = arrTabs(i).Substring(1)
                        AddHandler tab.Click, AddressOf tab_Click

                        ' Step 6: Attach event handler that will execute when 
                        ' user clicks on tab link 

                        ' Step 7: Set any other properties as desired 
                        tab.Width = System.Web.UI.WebControls.Unit.Pixel(_tabWidth - 2)
                        tab.Style("text-align") = "center"
                        tab.Style("font-size") = "larger"

                        ' Step 8: Insert tab <A> element into <TD> element 
                        tc.Controls.Add(tab)

                        ' Step 9: Insert <TD> element into <TR> element 
                        tr.Cells.Add(tc)
                        tabCount += 1
                    End If
                Next

                ' Add final blank cell to cause horizontal line to 
                ' run across entire zone width 
                tc = New TableCell()
                tc.Text = "&nbsp;"
                tc.Width = System.Web.UI.WebControls.Unit.Pixel(_tabWidth * 10)
                tc.Style("border-bottom") = "black 1px solid"
                tr.Cells.Add(tc)

                ' Step 10: Insert the <TR> element into <TABLE> and 
                ' add the HTML table to the page 
                tbl.Rows.Add(tr)

                Me.Controls.Add(tbl)
            Catch ex As Exception
                Dim lbl As New Label()
                lbl.Text = "Error: " + ex.Message
                Me.Controls.Add(lbl)
            End Try
        End Sub

        Protected Overloads Overrides Sub RenderContents(ByVal writer As System.Web.UI.HtmlTextWriter)
            If _debug Then
                writer.Write("Tab Data: " + _tabData + "<hr/>")
            End If
            ShowHideWebParts(writer)
            MyBase.RenderContents(writer)
        End Sub

        ' Show web parts for currently selected tab, 
        ' hide all others 
        Private Sub ShowHideWebParts(ByVal writer As System.Web.UI.HtmlTextWriter)
            Try
                Dim lbl As New Label()
                Dim arrTabs As String() = _tabData.Split(";"c)

                ' Step 1: If a tab has not been selected, assume 
                ' the first one 
                If _selectedTab Is Nothing Then
                    _selectedTab = arrTabs(0).Substring(1)
                End If

                ' Step 2: Hide all web parts in zone that are 
                ' below the ZoneTab part 
                For Each wp As WebPart In Me.Zone.WebParts
                    If wp.ZoneIndex > Me.ZoneIndex Then
                        wp.Hidden = True
                    End If
                Next
                For i As Integer = 0 To arrTabs.Length - 1

                    ' Step 3: Get web part names associated with this tab 
                    ' Step 4: Find the selected tab 
                    If arrTabs(i) = "*" + _selectedTab Then
                        For j As Integer = i + 1 To arrTabs.Length - 1
                            ' Step 5: Get associated web part names 
                            ' Step 6: Loop until next tab name found, or end of list 
                            If arrTabs(j).IndexOf("*") <> 0 Then
                                ' Step 7: Show named web parts 
                                For Each wp As WebPart In Me.Zone.WebParts
                                    If wp.Title = arrTabs(j) Then
                                        wp.Hidden = False
                                    End If
                                Next
                            Else
                                Exit For
                            End If
                        Next
                        ' Step 8: Bring tab border to "front" 
                        Dim tc As TableCell = DirectCast(Me.FindControl("tc_" + arrTabs(i).Substring(1).Replace(" ", "_")), TableCell)
                        tc.Style("border-bottom") = "white 1px solid"
                        tc.Style("border-top") = "black 1px solid"
                        tc.Style("border-left") = "black 1px solid"
                        tc.Style("border-right") = "black 1px solid"
                        tc.Style("background-color") = _tabBackgroundColorSelected

                        ' Step 9: Highlight selected tab 
                        Dim tab As LinkButton = DirectCast(Me.FindControl("tab_" + arrTabs(i).Substring(1).Replace(" ", "_")), LinkButton)
                        tab.Style("background-color") = _tabBackgroundColorSelected
                    Else
                        If arrTabs(i).IndexOf("*") = 0 Then
                            ' Step 10: Send tab border to "back" 
                            Dim tc As TableCell = DirectCast(Me.FindControl("tc_" + arrTabs(i).Substring(1).Replace(" ", "_")), TableCell)
                            tc.Style("border-bottom") = "black 1px solid"
                            tc.Style("border-top") = "gray 1px solid"
                            tc.Style("border-left") = "gray 1px solid"
                            tc.Style("border-right") = "gray 1px solid"
                            tc.Style("background-color") = _tabBackgroundColorDeselected

                            ' Step 11: Lowlight selected tab 
                            Dim tab As LinkButton = DirectCast(Me.FindControl("tab_" + arrTabs(i).Substring(1).Replace(" ", "_")), LinkButton)
                            tab.Style("background-color") = _tabBackgroundColorDeselected
                        End If
                    End If
                Next
            Catch ex As Exception
                writer.Write("Error: " + ex.Message)
            End Try
        End Sub

        ' This is the click event handler that was assigned 
        ' to all tab LinkButton objects in CreateChildControls() 
        ' method. 
        Private Sub tab_Click(ByVal sender As Object, ByVal e As EventArgs)
            Try
                ' Set flag indicated current tab, for 
                ' use in RenderContents() method 
                Dim tab As LinkButton = DirectCast(sender, LinkButton)
                _selectedTab = tab.Text
            Catch ex As Exception
                Dim lbl As New Label()
                lbl.Text = ex.Message
                Me.Controls.Add(lbl)
            End Try
        End Sub

    End Class
End Namespace