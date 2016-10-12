Imports System
Imports System.Data
Imports System.Configuration
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports System.Xml
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

    ' Get a sorted list of available list templates 
    Protected Sub cmdLookupListTemplates_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdLookupListTemplates.Click
        Dim objWebs As New WebsService.Webs()
        objWebs.Url = txtSiteUrl.Text + "/" + txtWebName.Text + "/_vti_bin/Webs.asmx"
        objWebs.Credentials = System.Net.CredentialCache.DefaultCredentials

        Dim xnListTemplates As XmlNode
        xnListTemplates = objWebs.GetListTemplates()

        ' Get sorted list of available list templates 
        Dim arrListItems As New ArrayList()
        For Each xnListTemplate As XmlNode In xnListTemplates.ChildNodes
            Try
                If xnListTemplate.Attributes("Hidden").Value.ToString() <> "TRUE" Then
                    arrListItems.Add(xnListTemplate.Attributes("DisplayName").Value + ":" + xnListTemplate.Attributes("Type").Value)
                End If
            Catch
                arrListItems.Add(xnListTemplate.Attributes("DisplayName").Value + ":" + xnListTemplate.Attributes("Type").Value)

            End Try
        Next
        arrListItems.Sort()

        ' Add them to the drop-down list 
        ddlListType.Items.Clear()
        Dim li As ListItem
        For Each templateData As String In arrListItems
            li = New ListItem(templateData.Split(":"c)(0), templateData.Split(":"c)(1))
            ddlListType.Items.Add(li)
        Next
        ddlListType.SelectedIndex = 0

        ' Show the rest of the form 
        Panel1.Visible = True

    End Sub

    ' Add the new list 
    Protected Sub cmdCreateList_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCreateList.Click
        Try
            lblErrorMsg.Visible = False

            ' Step 1: Create an instance of a list service 
            Dim objLists As New ListsService.Lists()
            objLists.Url = txtSiteUrl.Text + "/" + txtWebName.Text + "/_vti_bin/Lists.asmx"
            objLists.Credentials = System.Net.CredentialCache.DefaultCredentials

            ' Step 2: Create the new list 
            Dim listTemplateType As Integer = Integer.Parse(ddlListType.SelectedValue)
            objLists.AddList(txtListName.Text, "", listTemplateType)

            ' Step 3: Add any user defined fields - this requires 
            ' a bit of CAML 
            Dim xmlDoc As New XmlDocument()
            Dim xnNewFields As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "Fields", "")

            If txtUDFName1.Text <> "" Then
                xnNewFields.InnerXml += "<Method ID='1'>" + "<Field Type='" + ddlUDFType1.SelectedValue + "' DisplayName='" + txtUDFName1.Text + "'/>" + "</Method>"
            End If
            If txtUDFName2.Text <> "" Then
                xnNewFields.InnerXml += "<Method ID='2'>" + "<Field Type='" + ddlUDFType2.SelectedValue + "' DisplayName='" + txtUDFName2.Text + "'/>" + "</Method>"
            End If
            If txtUDFName3.Text <> "" Then
                xnNewFields.InnerXml += "<Method ID='3'>" + "<Field Type='" + ddlUDFType3.SelectedValue + "' DisplayName='" + txtUDFName3.Text + "'/>" + "</Method>"
            End If
            ' We can pass "null" values for any parameters we don't need to change, 
            ' so we're only passing data for the new fields we want to add 
            objLists.UpdateList(txtListName.Text, Nothing, xnNewFields, Nothing, Nothing, Nothing)

            ' Step 4: Add any new fields to the default view 
            Dim objViews As New ViewsService.Views()
            objViews.Url = txtSiteUrl.Text + "/" + txtWebName.Text + "/_vti_bin/Views.asmx"
            objViews.Credentials = System.Net.CredentialCache.DefaultCredentials

            ' Get a handle to the view 
            Dim xnDefaultView As XmlNode = objViews.GetView(txtListName.Text, "")

            ' Get the GUID of the view, which we'll need when we call the 
            ' UpdateView() method below 
            Dim viewName As String = xnDefaultView.Attributes("Name").Value

            ' Get any existing fields in the view, so we can add the new fields 
            ' to that list. To do this we need to find the "ViewFields" 
            ' node (if one exists), and grab it's XML to use as a starting point. 
            Dim xnViewFields As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "ViewFields", "")
            For Each childNode As XmlNode In xnDefaultView.ChildNodes
                If childNode.Name = "ViewFields" Then
                    xnViewFields.InnerXml += childNode.InnerXml
                End If
            Next

            ' Now add the new fields to end of the list of pre-existing 
            ' view fields. 
            If txtUDFName1.Text <> "" Then
                xnViewFields.InnerXml += "<FieldRef Name='" + txtUDFName1.Text + "'/>"
            End If
            If txtUDFName2.Text <> "" Then
                xnViewFields.InnerXml += "<FieldRef Name='" + txtUDFName2.Text + "'/>"
            End If
            If txtUDFName3.Text <> "" Then
                xnViewFields.InnerXml += "<FieldRef Name='" + txtUDFName3.Text + "'/>"
            End If

            ' Update the view. As with the ListUpdate() method, we only need to pass 
            ' parameters for data we want to change. We can pass "null" values for 
            ' all the rest. 
            objViews.UpdateView(txtListName.Text, viewName, Nothing, Nothing, xnViewFields, Nothing, _
            Nothing, Nothing)

            ' Step 5: If requested, open new list 
            If cbOpenList.Checked Then
                Response.Redirect(txtSiteUrl.Text.ToString() + "/" + txtWebName.Text.ToString() + "/lists/" + txtListName.Text.ToString())
            Else
                ' Step 6: Display success message 
                Me.RegisterClientScriptBlock("Success", "<script>alert ('List successfuly added');</script>")
            End If
        Catch ex As Exception
            lblErrorMsg.Text = ex.Message
            lblErrorMsg.Visible = True
        End Try
    End Sub

    ' Set standard type values for UDF type ddl's 
    Private Sub InitializeTypeDDL(ByRef ddl As DropDownList)
        ddl.Items.Clear()
        ddl.Items.Add("DateTime")
        ddl.Items.Add("Number")
        ddl.Items.Add("Text")
        ddl.SelectedIndex = 2
    End Sub

End Class