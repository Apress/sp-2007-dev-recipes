Imports System
Imports System.Data
Imports System.Configuration
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports System.Text
Imports System.Xml

Partial Public Class _Default
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not IsPostBack Then
            ' Populate the "Command" drop-down list 
            ddlCommand.Items.Add("Delete")
            ddlCommand.Items.Add("New")
            ddlCommand.Items.Add("Update")
            ddlCommand.SelectedIndex = 2

            ' Populate the "ID" drop-down list 
            ddlID = GetIDs()

            ' Get Current entries in list 
            RefreshEmployeeList()
        End If
    End Sub

    Protected Sub ddlCommand_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        ddlID = GetIDs()
    End Sub

    Private Function GetAllEmployees() As DataTable
        Dim objListService As New ListsService.Lists()
        objListService.Url = "http://localhost/_vti_bin/lists.asmx"
        objListService.Credentials = System.Net.CredentialCache.DefaultCredentials

        Dim dtEmployees As New DataTable("Employees")
        dtEmployees.Columns.Add("ID")
        dtEmployees.Columns.Add("EmpName")
        dtEmployees.Columns.Add("JobTitle")
        dtEmployees.Columns.Add("HireDate")
        Dim drEmployee As DataRow
        Dim xnEmployees As XmlNode = objListService.GetListItems("Employee", Nothing, Nothing, Nothing, Nothing, Nothing, _
        Nothing)
        For Each xnEmployee As XmlNode In xnEmployees.ChildNodes(1).ChildNodes
            Try
                drEmployee = dtEmployees.NewRow()
                drEmployee("ID") = xnEmployee.Attributes("ows_ID").Value
                drEmployee("EmpName") = xnEmployee.Attributes("ows_EmpName").Value
                drEmployee("JobTitle") = xnEmployee.Attributes("ows_JobTitle").Value
                drEmployee("HireDate") = xnEmployee.Attributes("ows_HireDate").Value
                dtEmployees.Rows.Add(drEmployee)
            Catch
            End Try
        Next

        Return dtEmployees

    End Function

    ' Return a drop-down list object containing 
    ' all current IDs, unless the "New" command 
    ' selected, in which case the only valud 
    ' value for ID is also "New" 
    Private Function GetIDs() As DropDownList
        ddlID.Items.Clear()
        If ddlCommand.SelectedValue = "New" Then
            Dim li As New ListItem("New", "New")
            ddlID.Items.Add(li)
        Else
            Dim dtEmployess As New DataTable()
            dtEmployess = GetAllEmployees()
            For Each drEmployee As DataRow In dtEmployess.Rows
                Dim li As New ListItem(drEmployee("ID").ToString(), drEmployee("ID").ToString())
                ddlID.Items.Add(li)
            Next
        End If
        Return ddlID
    End Function

    ' Redraw grid-view listing all employees 
    Private Sub RefreshEmployeeList()
        Dim dtEmployeeListData As New DataTable()
        dtEmployeeListData = GetAllEmployees()
        Me.GridView1.DataSource = dtEmployeeListData
        Me.GridView1.DataBind()
    End Sub

    ' Build necessary batch XML and call the web service method 
    Private Sub UpdateListWS(ByVal listName As String, ByVal dtListData As DataTable)

        ' Step 1: create a reference to the "Lists" web service 
        Dim objListService As New ListsService.Lists()
        objListService.Url = "http://localhost/_vti_bin/lists.asmx"
        objListService.Credentials = System.Net.CredentialCache.DefaultCredentials

        ' Step 2: loop through rows in data table, 
        ' adding one add, edit or delete command for each row 
        Dim drListItem As DataRow
        Dim strBatch As String = ""
        For i As Integer = 0 To dtListData.Rows.Count - 1
            drListItem = dtListData.Rows(i)

            ' Step 3: create a "Method" element to add to batch 
            ' Assume that first column of data table was the 'Cmd' 
            strBatch += "<Method ID='" + i.ToString() + "' Cmd='" + drListItem("Cmd") + "'>"
            For j As Integer = 1 To drListItem.Table.Columns.Count - 1

                ' Step 4: loop through fields 2-n, building 
                ' one "method" in batch 
                ' Only include columns with data 
                If drListItem(j).ToString() <> "" Then
                    strBatch += "<Field Name='" + drListItem.Table.Columns(j).ColumnName + "'>"
                    strBatch += Server.HtmlEncode(drListItem(j).ToString())
                    strBatch += "</Field>"
                End If
            Next

            ' Step 5: close out this method entry 
            strBatch += "</Method>"
        Next

        ' Step 6: Create the parent "batch" element 
        Dim xmlDoc As XmlDocument = New System.Xml.XmlDocument()
        Dim xmlBatch As System.Xml.XmlElement = xmlDoc.CreateElement("Batch")

        ' Step 7: tell SharePoint to keep processing if a single 
        ' "Method" causes an error. 
        xmlBatch.SetAttribute("OnError", "Continue")
        xmlBatch.SetAttribute("ListVersion", "1")
        xmlBatch.SetAttribute("ViewName", "")

        ' Step 8: add method (i.e. add/update/delete command) to batch 
        xmlBatch.InnerXml = strBatch

        ' Step 9: process the batch 
        Dim xmlReturn As XmlNode = objListService.UpdateListItems(listName, xmlBatch)

        ' Display batch that was just run on web page 
        lblBatchXML.Text = "<strong>Batch just processed</strong><br/><br/>" + Server.HtmlEncode(xmlBatch.OuterXml)

        'Display the returned results 
        lblReturnXML.Text = "<strong>Results</strong><br/><br/>" + Server.HtmlEncode(xmlReturn.InnerXml)

    End Sub

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs)

        ' Define table to hold data to process 
        Dim dtEmployees As New DataTable("Employee")

        ' New, Update, or Delete 
        dtEmployees.Columns.Add("Cmd")

        ' New if adding, or ID of item 
        dtEmployees.Columns.Add("ID")
        ' Builtin Title column 
        dtEmployees.Columns.Add("Title")
        ' Employee name 
        dtEmployees.Columns.Add("EmpName")
        ' Employee hire date 
        dtEmployees.Columns.Add("HireDate")
        ' Employee title 
        dtEmployees.Columns.Add("JobTitle")

        ' Call routine to update list 
        Dim drEmployee As DataRow = dtEmployees.NewRow()
        drEmployee("Cmd") = ddlCommand.SelectedValue
        drEmployee("ID") = ddlID.SelectedValue
        drEmployee("EmpName") = txtEmpName.Text
        drEmployee("JobTitle") = txtJobTitle.Text
        drEmployee("HireDate") = txtHireDate.Text
        dtEmployees.Rows.Add(drEmployee)

        ' Update SharePoint 
        UpdateListWS("Employee", dtEmployees)

        RefreshEmployeeList()

    End Sub

End Class