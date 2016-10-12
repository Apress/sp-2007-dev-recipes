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
        Dim siteUrl As String
        Dim webName As String = ""
        Dim fontSize As Integer = 14

        ' Validate site url 
        Try
            siteUrl = Request.QueryString("siteUrl").ToString()
        Catch
            Response.Write("<div style='font-size: " & fontSize & "pt; font-weight: bold; font-color: red'>")
            Response.Write("Please provide a value for 'siteUrl' in the querystring")
            Response.Write("</div>")
            Return
        End Try

        ' Validate web name 
        Try
            webName = Request.QueryString("webName").ToString()
        Catch
            Response.Write("<div style='font-size: " + fontSize + "pt; font-weight: bold; font-color: red'>")
            Response.Write("Please provide a value for 'webName' in the querystring")
            Response.Write("</div>")
            Return
        End Try
        '} 

        ' Display banner and return link 
        Response.Write("<H1>List of users for web site '" & siteUrl & "/" + webName + "</H1>")

        Dim site As New SPSite(siteUrl)
        Dim ADGroups As New ArrayList()
        Dim userList As New DataTable()
        userList = GetAllSiteUsers(siteUrl, webName)

        Dim heading As String = "The following report displays all members of the '" & site.AllWebs(webName).Title & "' Site as of " + DateTime.Now.ToLocalTime() + "<br/><br/>"

        Response.Write("<div style='font-size: " & fontSize & "; font-weight: bold'>")
        Response.Write(heading)
        Response.Write("<hr/>")
        Response.Write("</div>")

        ' Display users in all groups, and who are not members of any group. 
        Dim prevGroup As String = "..."
        Dim table As New Table()

        For Each userRow As DataRow In userList.Rows

            ' If a new GROUP, display heading info 
            If prevGroup <> DirectCast(userRow("GroupName"), String) Then
                Response.Write("<br/>")
                If DirectCast(userRow("GroupName").ToString(), String) <> "" Then
                    Response.Write("<strong>Group: " & userRow("GroupName") & " [" + userRow("GroupRoles") & "]</strong><br/><br/>")
                Else
                    Response.Write("<strong>The following users have been given explicit permissions in this Site</strong><br/><br/>")
                End If
                prevGroup = DirectCast(userRow("GroupName"), String)
            End If

            If userRow("UserName").ToString() <> "" Then
                Response.Write(userRow("UserName"))
            Else
                Response.Write(" (" + userRow("UserAlias") + ") ")
            End If

            If DirectCast(userRow("UserRoles"), String) <> "" Then
                Response.Write(" [" + userRow("UserRoles") + "] ")
            End If

            If DirectCast(userRow("IsADGroup"), String) <> "False" Then
                Response.Write("<font color='red'> ** Active Directory Security Group</font>")
            End If
            Response.Write("<br/>")
        Next
        Response.Write("</div>")
    End Sub

    Private Function GetAllSiteUsers(ByVal siteUrl As String, ByVal webName As String) As DataTable
        ' Step1: Open the web site to process 
        Dim site As New SPSite(siteUrl)
        Dim web As SPWeb = site.AllWebs(webName)

        ' Step 2: Create a data table to hold list 
        ' of site users 
        Dim userList As New DataTable("UserList")
        Dim userRow As DataRow
        userList.Columns.Add("GroupName")
        userList.Columns.Add("GroupRoles")
        userList.Columns.Add("UserAlias")
        userList.Columns.Add("UserName")
        userList.Columns.Add("UserRoles")
        userList.Columns.Add("UserCompany")
        userList.Columns.Add("IsADGroup")

        ' Step 3: Iterate through site groups 
        For Each group As SPGroup In web.SiteGroups
            ' Step 4:Get list of all users in this group 
            ' and add to data table 
            For Each user As SPUser In group.Users
                userRow = userList.NewRow()

                userRow("GroupName") = group.Name
                userRow("GroupRoles") = GetRoles(group)
                userRow("UserName") = user.Name
                userRow("UserAlias") = user.LoginName.ToString()
                userRow("UserRoles") = GetRoles(user)
                userRow("IsADGroup") = user.IsDomainGroup.ToString()

                userList.Rows.Add(userRow)
            Next
        Next

        ' Step 5: Get users who have been assigned 
        ' explicit permissions 
        For Each user As SPUser In web.Users
            If user.Groups.Count = 0 OrElse GetRoles(user) <> "" Then
                userRow = userList.NewRow()

                userRow("GroupName") = ""
                userRow("GroupRoles") = ""
                userRow("UserName") = user.Name
                userRow("UserAlias") = user.LoginName
                userRow("UserRoles") = GetRoles(user)
                userRow("IsADGroup") = user.IsDomainGroup.ToString()

                userList.Rows.Add(userRow)
            End If
        Next

        Return userList
    End Function

    ' Note: the SPUser.Roles collection has been 
    ' deprecated in WSS 3.0, but it's still the 
    ' simplest way to access roles assigned to a 
    ' user. 
    Private Function GetRoles(ByVal gu As SPPrincipal) As String
        Dim roleInfo As String = ""
        For Each role As SPRole In gu.Roles
            If roleInfo <> "" Then
                roleInfo = roleInfo + ","
            End If
            roleInfo = roleInfo + role.Name.ToString()
        Next
        Return roleInfo
    End Function
End Class
