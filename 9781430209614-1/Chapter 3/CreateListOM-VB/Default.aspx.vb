Imports System
Imports System.Data
Imports System.Configuration
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports Microsoft.SharePoint
Imports System.Collections

Partial Public Class _Default
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ' Get column types 
            InitializeTypeDDL(ddlUDFType1)
            InitializeTypeDDL(ddlUDFType2)
            InitializeTypeDDL(ddlUDFType3)
        End If

    End Sub
    Protected Sub cmdCreateList_Click(ByVal sender As Object, ByVal e As EventArgs)
        Try
            lblErrorMsg.Visible = False

            ' Step 1: get a handle to the site collection and web site 
            Dim site As New SPSite(txtSiteUrl.Text)
            Dim web As SPWeb = site.AllWebs(txtWebName.Text)
            Dim listCollection As SPListCollection = web.Lists
            web.AllowUnsafeUpdates = True

            ' Step 2: Create the new list 
            listCollection.Add(txtListName.Text, "", web.ListTemplates(ddlListType.SelectedItem.Text))
            Dim newList As SPList = listCollection(txtListName.Text)

            ' Step 3: Add any user defined fields 
            If txtUDFName1.Text <> "" Then
                AddField(newList, txtUDFName1, ddlUDFType1)
            End If
            If txtUDFName2.Text <> "" Then
                AddField(newList, txtUDFName2, ddlUDFType2)
            End If
            If txtUDFName3.Text <> "" Then
                AddField(newList, txtUDFName3, ddlUDFType3)
            End If

            ' Step 4: Save the changes 
            newList.Update()
            web.Update()

            ' Step 5: If requested, open new list 
            If cbOpenList.Checked Then
                Response.Redirect(web.Url.ToString() + "/lists/" + newList.Title.ToString())
            Else
                ' Step 6: Display success message 
                Me.RegisterClientScriptBlock("Success", "<script>alert ('List successfuly added');</script>")
            End If
        Catch ex As Exception
            lblErrorMsg.Text = ex.Message
            lblErrorMsg.Visible = True
        End Try
    End Sub

    ' Add the UDF to list and default view 
    Private Sub AddField(ByVal newList As SPList, ByVal tb As TextBox, ByVal ddl As DropDownList)
        Dim defaultView As SPView = newList.DefaultView
        newList.Fields.Add(tb.Text, GetFieldType(ddl), False)
        Dim newField As SPField = newList.Fields(tb.Text)
        defaultView.ViewFields.Add(newField)
        defaultView.Update()
    End Sub

    ' Return SP field type from ddl value for UDF type 
    Private Function GetFieldType(ByVal ddlUDFType As DropDownList) As SPFieldType
        Select Case ddlUDFType.SelectedItem.Value
            Case ("Number")
                Return SPFieldType.Number
            Case ("Text")
                Return SPFieldType.Text
            Case ("Date")
                Return SPFieldType.DateTime
            Case Else
                Return SPFieldType.Text
        End Select
    End Function

    ' Get a sorted list of all templates available 
    Protected Sub cmdLookupListTemplates_Click(ByVal sender As Object, ByVal e As EventArgs)
        Try
            lblErrorMsg.Visible = False

            Dim site As New SPSite(txtSiteUrl.Text)
            Dim web As SPWeb = site.AllWebs(txtWebName.Text)

            ' Get sorted list of available list templates 
            Dim arrListItems As New ArrayList()
            For Each listTemplate As SPListTemplate In web.ListTemplates
                If Not listTemplate.Hidden Then
                    arrListItems.Add(listTemplate.Name)
                End If
            Next
            arrListItems.Sort()

            ' Add them to the drop-down list 
            ddlListType.Items.Clear()
            Dim li As ListItem
            For Each templateName As String In arrListItems
                ddlListType.Items.Add(templateName)
            Next
            ddlListType.SelectedIndex = 0

            ' Show the rest of the form 

            Panel1.Visible = True
        Catch ex As Exception
            lblErrorMsg.Text = ex.Message
            lblErrorMsg.Visible = True
        End Try
    End Sub

    ' Set standard type values for UDF type ddl's 
    Private Sub InitializeTypeDDL(ByRef ddl As DropDownList)
        ddl.Items.Clear()
        ddl.Items.Add("Date")
        ddl.Items.Add("Number")
        ddl.Items.Add("Text")
        ddl.SelectedIndex = 2
    End Sub

End Class