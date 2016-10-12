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
Imports System.Xml

Partial Public Class _Default
    Inherits System.Web.UI.Page

    Private objUserGroup As New UserGroupService.UserGroup()

    Private Function GetPermissionFlags(ByVal strRoleName As String) As ULong

        ' Step 1: Default to NO permissions 
        Dim permissionFlags As ULong = CLng(SPBasePermissions.EmptyMask)

        ' Step 2: Get list of all current roles for this web site 
        Dim xnRoles As XmlNode = objUserGroup.GetRoleCollectionFromWeb()

        ' Step 3: Even though we're using the web service to update 
        ' the roles collection, we can use the built-in enum 
        ' type to get the numeric values of the various base 
        ' permissions. 
        Dim enumBasePermissions As New SPBasePermissions()
        Dim arrBasePermissionNames As String() = System.Enum.GetNames(enumBasePermissions.GetType())
        Dim arrBasePermissionValues As ULong() = System.Enum.GetValues(enumBasePermissions.GetType())

        ' Step 4: loop through all current roles in target site 
        ' finding the role for which we want to duplicate permission 
        ' flags. 
        For Each xnRole As XmlNode In xnRoles.FirstChild.ChildNodes
            If xnRole.Attributes("Name").Value.ToString().ToLower() = strRoleName.ToLower() Then
                ' Turn the comma-delimited list of base permissing names into 
                ' an array so we can iterate through them 
                Dim arrPermission As String() = xnRole.Attributes("BasePermissions").Value.ToString().Split(","c)

                ' Iterate through the complete list of base permissions to find the entry 
                ' that matches the base permission from our template role 
                For i As Integer = 0 To arrPermission.Length - 1
                    For j As Integer = 0 To arrBasePermissionNames.Length - 1

                        ' When we've found our base permission, "OR" its 
                        ' numeric value with that of any other base permissions 
                        ' to create the complete set of values 
                        If arrPermission(i).Trim() = arrBasePermissionNames(j) Then
                            permissionFlags = permissionFlags Or arrBasePermissionValues(j)
                        End If

                    Next
                Next

            End If

        Next

        Return permissionFlags

    End Function

    Protected Sub cmdAddRole_Click(ByVal sender As Object, ByVal e As EventArgs)

        Try

            ' Point the UserGroup web service to our target site collection 
            ' and web site 
            objUserGroup.Url = txtSiteUrl.Text + "/" + txtWebName.Text + "/_vti_bin/usergroup.asmx"
            objUserGroup.Credentials = System.Net.CredentialCache.DefaultCredentials

            ' Get the permission flags of the role to be cloned 
            Dim permissionFlags As ULong = GetPermissionFlags(rblTemplateRole.SelectedValue)

            ' Create the new role 
            objUserGroup.AddRoleDef(txtRoleName.Text, txtRoleDefinition.Text, permissionFlags)

            ' Display success message 
            lblReturnMsg.Text = "Successfully added '" + txtRoleName.Text + "' role."

        Catch ex As Exception
            lblReturnMsg.Text = "Error: " + ex.Message
        End Try

    End Sub

End Class