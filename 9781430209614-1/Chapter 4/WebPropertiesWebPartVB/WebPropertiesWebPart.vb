Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls
Imports Microsoft.SharePoint.Utilities

Public Class WebPropertiesWebPart
    Inherits WebPart
    ' Define location variables 
    Private _debug As Boolean = False
    Private _prefix As String = "_mg"

    <Personalizable()> _
    <WebBrowsable()> _
    <WebDescription("Check to display debug information")> _
    <WebDisplayName("Debug?")> _
    Public Property Debug() As Boolean
        Get
            Return _debug
        End Get
        Set(ByVal value As Boolean)
            _debug = value
        End Set
    End Property

    <Personalizable()> _
    <WebBrowsable()> _
    <WebDescription("Prefix to use when storing and retrieving properties")> _
    <WebDisplayName("Property prefix: ")> _
    Public Property Prefix() As String
        Get
            Return _prefix
        End Get
        Set(ByVal value As String)
            _prefix = value
        End Set
    End Property

    Protected Overloads Overrides Sub CreateChildControls()
        MyBase.CreateChildControls()

        Try
            ' Step 1: Create table to hold existing, new properties 
            Dim tbl As New Table()
            Dim tr As TableRow
            Dim tc As TableCell
            Dim delBtn As ImageButton
            Dim tb As TextBox
            Dim lbl As Label
            Dim i As Integer = 1

            ' Just a bit of formatting 
            tbl.CellPadding = 3
            tbl.CellSpacing = 3
            tbl.BorderStyle = BorderStyle.Solid

            ' Add a heading 
            tr = New TableRow()
            ' # 
            tc = New TableCell()
            tr.Cells.Add(tc)
            ' Key 
            tc = New TableCell()
            tc.Text = "Property Key"
            tc.Font.Bold = True
            tr.Cells.Add(tc)
            ' Value 
            tc = New TableCell()
            tc.Text = "Value"
            tc.Font.Bold = True
            tr.Cells.Add(tc)
            tbl.Rows.Add(tr)
            ' Delete button 
            tc = New TableCell()
            tr.Cells.Add(tc)

            tc.Font.Bold = True
            tr.Cells.Add(tc)
            tbl.Rows.Add(tr)

            ' Step 2: Loop through existing properties that match prefix 
            ' and are not null, add to table 
            Dim web As SPWeb = SPControl.GetContextWeb(Context)
            Dim properties As SPPropertyBag = web.Properties
            Dim isAdmin As Boolean = web.CurrentUser.IsSiteAdmin

            For Each key As Object In properties.Keys
                If key.ToString().IndexOf(_prefix) = 0 AndAlso properties(key.ToString()) IsNot Nothing Then
                    ' Create a new row for current property 
                    tr = New TableRow()

                    ' # 
                    tc = New TableCell()
                    tc.Text = i.ToString() + ". "
                    tr.Cells.Add(tc)

                    ' Key 
                    tc = New TableCell()
                    tc.Text = key.ToString().Substring(_prefix.Length)
                    tc.ID = "key_" + i.ToString()
                    tr.Cells.Add(tc)

                    ' Value 
                    tc = New TableCell()

                    ' 3. For admin users, show value in 
                    ' an editable text box + delete button 
                    If isAdmin Then
                        tb = New TextBox()
                        tb.Text = properties(key.ToString())
                        tb.ID = "value_" + i.ToString()
                        tc.Controls.Add(tb)
                        tr.Cells.Add(tc)

                        tc = New TableCell()
                        delBtn = New ImageButton()
                        delBtn.ImageUrl = "/_layouts/images/delete.gif"
                        AddHandler delBtn.Click, AddressOf delBtn_Click
                        delBtn.ID = "delete_" + i.ToString()
                        tc.Controls.Add(delBtn)
                        tr.Cells.Add(tc)
                    Else
                        ' for non-admin users, just show read-only 
                        lbl = New Label()
                        lbl.Text = properties(key.ToString())
                        tc.Controls.Add(lbl)
                        tr.Cells.Add(tc)
                    End If

                    ' Add new row to table 
                    tbl.Rows.Add(tr)
                    i += 1
                End If
            Next

            ' Step 4: Add a final row to allow user 
            ' to add new properties if current user is site admin 
            If isAdmin Then
                tr = New TableRow()

                ' # 
                tc = New TableCell()
                tc.Text = "*. "
                tr.Cells.Add(tc)

                ' Key 
                tc = New TableCell()
                tb = New TextBox()
                tb.Text = ""
                tb.ID = "key_new"
                tc.Controls.Add(tb)
                tr.Cells.Add(tc)

                ' Value 
                tc = New TableCell()
                tb = New TextBox()
                tb.Text = ""
                tb.ID = "value_new"
                tc.Controls.Add(tb)
                tr.Cells.Add(tc)

                tbl.Rows.Add(tr)
            End If

            ' Step 5: Add the completed table to the page 
            Me.Controls.Add(tbl)

            ' Step 6: Now add a button to save changes, 
            ' if current user is site admin 
            If isAdmin Then
                lbl = New Label()
                lbl.Text = "<br/>"
                Me.Controls.Add(lbl)
                Dim btn As New Button()
                btn.Text = "Save changes"
                AddHandler btn.Click, AddressOf btn_Click
                Me.Controls.Add(btn)

            End If
        Catch ex As Exception
            Dim lbl As New Label()
            lbl.Text = "Error: " + ex.Message
            Me.Controls.Add(lbl)
        End Try

    End Sub

    ' Handles "Save Changes" button click event 
    Private Sub btn_Click(ByVal sender As Object, ByVal e As EventArgs)
        Try
            ' Step 1: Get handle to web site property 
            ' collection 
            Dim isChanged As Boolean = False
            Dim web As SPWeb = SPControl.GetContextWeb(Context)
            Dim properties As SPPropertyBag = web.Properties
            web.AllowUnsafeUpdates = True

            ' Step 2: Add new property 
            Dim tbNewKey As TextBox = DirectCast(Me.FindControl("key_new"), TextBox)
            Dim tbNewValue As TextBox = DirectCast(Me.FindControl("value_new"), TextBox)
            If tbNewKey.Text <> "" Then
                properties(_prefix + tbNewKey.Text) = tbNewValue.Text
                web.Properties.Update()
                isChanged = True
            End If

            ' Step 3: Loop through textboxes in web part 
            ' updating corresponding site property if 
            ' checkbox has been changed. 
            Dim tc As TableCell
            Dim tb As TextBox
            For i As Integer = 1 To 998
                tc = DirectCast(Me.FindControl("key_" + i.ToString()), TableCell)

                ' Step 4: If a control with the name "key_<n>" exists, get it, 
                ' otherwise assume no more custom properties to edit 
                If tc IsNot Nothing Then
                    ' Step 5: Ok, we found the textbox containing the property 
                    ' value, now let's see if the value in the textbox 
                    ' has been changed to something other than that in 
                    ' the corresponding web property. 
                    tb = DirectCast(Me.FindControl("value_" + i.ToString()), TextBox)
                    If properties(_prefix + tc.Text).Trim() <> tb.Text.Trim() Then
                        ' Step 6: The value was changed, update the web property 
                        ' and set the flag indicating that web part 
                        ' needs to be redrawn 
                        properties(_prefix + tc.Text) = tb.Text
                        web.Properties.Update()
                        isChanged = True
                    End If
                Else
                    Exit For
                End If
            Next

            ' Step 7: If any changes made, redraw web part 
            ' to reflect changed/added properties 
            If isChanged Then
                Me.Controls.Clear()
                CreateChildControls()
            End If

        Catch ex As Exception
            Dim lbl As New Label()
            lbl.Text = "<br/><br/>Error: " + ex.Message
            Me.Controls.Add(lbl)
        End Try

    End Sub

    ' Handles individual property delete button click 
    Private Sub delBtn_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
        Try
            ' Step 1. Get handle to name of property to be 
            ' deleted 
            Dim delBtn As ImageButton = DirectCast(sender, ImageButton)
            Dim _id As String = delBtn.ID.Replace("delete_", "")
            Dim tc As TableCell = DirectCast(Me.FindControl("key_" + _id), TableCell)

            ' Step 2: Get handle to web site, property collection 
            Dim web As SPWeb = SPControl.GetContextWeb(Context)
            Dim properties As SPPropertyBag = web.Properties
            web.AllowUnsafeUpdates = True

            ' Step 3: Delete the unwanted property by setting 
            ' it's value to null (note: for some reason using 
            ' the Remove() method was not sufficient to cause 
            ' SharePoint to delete the property. 
            web.Properties(_prefix + tc.Text) = Nothing
            web.Properties.Update()
            web.Update()

            ' Step 4: Refresh list 
            Me.Controls.Clear()
            CreateChildControls()

            ' Step 5: Display message to user informing them 
            ' that property has been deleted 
            Dim lbl As New Label()
            lbl.Text = "<br/><br/>You deleted property '" + tc.Text + "'"
            Me.Controls.Add(lbl)

        Catch ex As Exception
            Dim lbl As New Label()
            lbl.Text = "<br/><br/>Error: " + ex.Message
            Me.Controls.Add(lbl)
        End Try

    End Sub

    Protected Overloads Overrides Sub RenderContents(ByVal writer As System.Web.UI.HtmlTextWriter)
        MyBase.RenderContents(writer)

        If _debug Then
            writer.Write("<hr/>")
            writer.Write("<strong>Prefix:</strong> " + _prefix)
            writer.Write("<hr/>")
        End If
    End Sub

End Class
