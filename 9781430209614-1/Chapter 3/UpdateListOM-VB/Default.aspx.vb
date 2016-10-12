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

Partial Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ' Populate the "Command" drop-down list 
            ddlCommand.Items.Add("Delete")
            ddlCommand.Items.Add("New")
            ddlCommand.Items.Add("Update")
            ddlCommand.SelectedIndex = 2

            ' Get list of current ID's 
            GetIDs()

            ' Populate form fields 
            setFields()

            ' Get Current entries in list 
            RefreshEmployeeList()
        End If
    End Sub

    ' Return list of employees currently in 
    ' the SharePoint list 
    Private Function GetAllEmployees() As DataTable
        Dim site As New SPSite("http://localhost")
        Dim web As SPWeb = site.AllWebs("")
        Dim employees As SPList = web.Lists("Employee")

        Dim dtEmployees As DataTable = employees.Items.GetDataTable()
        Dim dtEmployeesNew As New DataTable("Employees")

        dtEmployeesNew.Columns.Add("ID")
        dtEmployeesNew.Columns.Add("EmpName")
        dtEmployeesNew.Columns.Add("JobTitle")
        dtEmployeesNew.Columns.Add("HireDate")

        For Each drEmployee As DataRow In dtEmployees.Rows
            Try
                Dim drEmployeeNew As DataRow = dtEmployeesNew.NewRow()
                drEmployeeNew("ID") = drEmployee("ID").ToString()
                drEmployeeNew("EmpName") = drEmployee("EmpName").ToString()
                drEmployeeNew("JobTitle") = drEmployee("JobTitle").ToString()
                drEmployeeNew("HireDate") = drEmployee("HireDate").ToString()
                dtEmployeesNew.Rows.Add(drEmployeeNew)
                dtEmployeesNew.AcceptChanges()
            Catch
            End Try
        Next

        Return dtEmployeesNew

    End Function

    ' Return a drop-down list object containing 
    ' all current IDs, unless the "New" command 
    ' selected, in which case no ID is needed 
    Private Function GetIDs() As DropDownList
        ddlID.Items.Clear()
        If ddlCommand.SelectedValue = "New" Then
            ddlID.Enabled = False
            ddlID.Items.Add(New ListItem("N/A"))
        Else
            ddlID.Enabled = True
            Dim dtEmployess As New DataTable()
            dtEmployess = GetAllEmployees()
            For Each drEmployee As DataRow In dtEmployess.Rows
                Dim li As New ListItem(drEmployee("ID").ToString(), drEmployee("ID").ToString())
                ddlID.Items.Add(li)
            Next
        End If
        ddlID.SelectedIndex = 0
        Return ddlID
    End Function

    ' Redraw grid-view listing all employees 
    Private Sub RefreshEmployeeList()
        Dim dtEmployeeListData As New DataTable()
        dtEmployeeListData = GetAllEmployees()
        Me.GridView1.DataSource = dtEmployeeListData
        Me.GridView1.DataBind()
    End Sub

    ' Update form fields to reflect 
    ' selected command and, if appropriate 
    ' selected ID 
    Private Sub setFields()
        ' Clear out data entry fields 
        txtEmpName.Text = ""
        txtHireDate.Text = ""
        txtJobTitle.Text = ""

        ' By default, let user select an existing ID 
        ddlID.Enabled = True

        ' Enable or disable fields as appropriate 
        If ddlCommand.SelectedValue = "Delete" Then
            txtEmpName.Enabled = False
            txtHireDate.Enabled = False
            txtJobTitle.Enabled = False
        Else
            ' If "New", doesn't make sense for 
            ' user to select an ID 
            If ddlCommand.SelectedValue = "New" Then
                ddlID.Enabled = False
            Else
                ddlID.Enabled = True

                ' Retrieve existing data for selected employee 
                Dim site As New SPSite("http://localhost")
                Dim web As SPWeb = site.AllWebs("")
                Dim list As SPList = web.Lists("Employee")
                Dim ID As Integer = Integer.Parse(ddlID.SelectedValue)
                Dim item As SPListItem = list.GetItemById(ID)

                ' Assign form field values from SharePoint list 
                txtEmpName.Text = item("EmpName").ToString()
                txtHireDate.Text = item("HireDate").ToString()
                txtJobTitle.Text = item("JobTitle").ToString()
            End If

            txtEmpName.Enabled = True
            txtHireDate.Enabled = True
            txtJobTitle.Enabled = True
        End If
    End Sub

    Protected Sub ddlCommand_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        GetIDs()
        setFields()
    End Sub

    Protected Sub ddlID_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        setFields()
    End Sub

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim site As New SPSite("http://localhost")
            Dim web As SPWeb = site.AllWebs("")
            Dim list As SPList = web.Lists("Employee")
            Dim item As SPListItem
            Dim ID As Integer
            lblReturnMsg.Text = ""
            lblReturnMsg.Visible = True

            web.AllowUnsafeUpdates = True

            Select Case ddlCommand.SelectedValue
                Case "New"
                    item = list.Items.Add()
                    item("EmpName") = txtEmpName.Text
                    item("JobTitle") = txtJobTitle.Text
                    item("HireDate") = txtHireDate.Text
                    item.Update()
                    lblReturnMsg.Text = "'" & txtEmpName.Text & "' has been successfuly added"
                    Exit Select
                Case "Update"
                    ID = Integer.Parse(ddlID.SelectedValue)
                    item = list.GetItemById(ID)
                    item("EmpName") = txtEmpName.Text
                    item("JobTitle") = txtJobTitle.Text
                    item("HireDate") = txtHireDate.Text
                    item.Update()
                    lblReturnMsg.Text = "'" & txtEmpName.Text & "' has been successfuly updated"
                    Exit Select
                Case "Delete"
                    ID = Integer.Parse(ddlID.SelectedValue)
                    item = list.GetItemById(ID)
                    Dim empName As String = item("EmpName").ToString()
                    list.Items.DeleteItemById(ID)
                    lblReturnMsg.Text = "'" & empName & "' has been successfuly deleted"
                    Exit Select
            End Select
            list.Update()

            GetIDs()
            setFields()
            RefreshEmployeeList()
        Catch ex As Exception
            lblReturnMsg.Text = ex.Message
        End Try
    End Sub

End Class