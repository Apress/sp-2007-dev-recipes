Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.SharePoint

Namespace AddRoleOM_VB

    Class Program

        Private Structure roleData
            Public SiteUr As String
            Public WebName As String
            Public RoleName As String
            Public RoleDescription As String
            Public PermissionMask As SPBasePermissions
        End Structure

        Public Shared Sub Main()

            'Step 1: Prompt data needed to define 
            ' the new role 
            Dim r As roleData = GetParams()

            Try

                'Step 2: Get handle to specifie site collection and web site 
                Dim site As New SPSite(r.SiteUr)
                Dim web As SPWeb = site.AllWebs(r.WebName)
                web.AllowUnsafeUpdates = True

                'Step 3: Get collection of current role definitions for site 
                Dim roles As SPRoleDefinitionCollection = web.RoleDefinitions

                'Step 4: Create a new role using information passed in 
                Dim role As New SPRoleDefinition()
                role.Name = r.RoleName
                role.Description = r.RoleDescription
                role.BasePermissions = r.PermissionMask
                roles.Add(role)

                'Step 5: Display success message 
                Console.WriteLine("Role '" + r.RoleName + "' has been successfully added")

            Catch ex As Exception

                'Step 6: If error occurred, display error message 
                Console.WriteLine(ex.Message)

            End Try

            Console.WriteLine()
            Console.WriteLine("Press any key to continue...")
            Console.Read()

        End Sub

        Private Shared Function GetParams() As roleData
            Dim r As New roleData()

            Try
                ' Get the basic data 
                Console.WriteLine()
                Console.Write("Site collection url: ")
                r.SiteUr = Console.ReadLine()
                Console.Write("Site (web): ")
                r.WebName = Console.ReadLine()
                Console.Write("Role to add: ")
                r.RoleName = Console.ReadLine()
                Console.Write("Description of role: ")
                r.RoleDescription = Console.ReadLine()

                ' Now get a character that represents the 
                ' set of permissions the new role should 
                ' inherit 
                Console.Write("Role (F=Full Control, C=Contribute, R=Read): ")
                Dim strBasePermission As String = Console.ReadLine()

                'Only allow user to enter valid permission character, 
                'keep looping until valid response provided 
                Dim site As New SPSite(r.SiteUr)
                Dim web As SPWeb = site.AllWebs(r.WebName)

                r.PermissionMask = SPBasePermissions.EmptyMask
                While True
                    Select Case strBasePermission.ToUpper()
                        Case "F"
                            r.PermissionMask = web.RoleDefinitions("Full Control").BasePermissions
                            Exit Select
                        Case "C"
                            r.PermissionMask = web.RoleDefinitions("Contribute").BasePermissions
                            Exit Select
                        Case "R"
                            r.PermissionMask = web.RoleDefinitions("Read").BasePermissions
                            Exit Select
                    End Select
                    If r.PermissionMask <> SPBasePermissions.EmptyMask Then
                        Exit While
                    Else
                        Console.Write("Your selection was not valid (F=Full Control, C=Contribute, R=Read): ")
                        strBasePermission = Console.ReadLine()
                    End If
                End While

                Console.WriteLine()
                Console.WriteLine()

            Catch ex As Exception

                ' If an error occurred, display the error message 
                Console.WriteLine()
                Console.WriteLine(ex.Message)
                Console.WriteLine()

            End Try

            Return r

        End Function

    End Class

End Namespace