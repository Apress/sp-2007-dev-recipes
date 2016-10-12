Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.SharePoint

Class Module1

    Public Shared Sub Main(ByVal args As String())

        'Step 1: If no arguments passed in, prompt for them now 
        If (args.Length = 0) Then
            args = New String(6) {}
            args = GetParams(args)
        End If

        Try

            'Step 3a: Get handle to specifie site collection and web site 
            Dim site As New SPSite(args(0))
            Dim web As SPWeb = site.AllWebs(args(1))

            'Step 3b: Add the specified site group 
            web.SiteGroups.Add(args(2), web.SiteUsers(args(3)), web.SiteUsers(args(4)), args(5))

            'Step 4: Assign specified role to the new group 
            'Note: even though the SPWeb.Roles collection has been flagged as 
            ' "obsolete" by Microsoft it is still the easieast way to 
            ' add roles to site groups in WSS/MOSS 2007 
            web.Roles(args(6)).AddGroup(web.SiteGroups(args(2)))

            'Step 5: Display success message 
            Console.WriteLine("Site group '" + args(2) + "' has been successfully added")
        Catch ex As Exception


            'Step 6: If error occurred, display error message 

            Console.WriteLine(ex.Message)
        End Try

        Console.WriteLine()
        Console.WriteLine("Press any key to continue...")
        Console.Read()

    End Sub

    Private Shared Function GetParams(ByVal args As String()) As String()
        Try

            ' Step 2: Get parameters from user 
            Console.WriteLine()
            Console.Write("Site collection url: ")
            args(0) = Console.ReadLine()
            Console.Write("Site (web): ")
            args(1) = Console.ReadLine()
            Console.Write("Site group to add: ")
            args(2) = Console.ReadLine()
            Console.Write("Owner login: ")
            args(3) = Console.ReadLine()
            Console.Write("Default user: ")
            args(4) = Console.ReadLine()
            Console.Write("Site group description: ")
            args(5) = Console.ReadLine()
            Console.Write("Role (Full Control/Contribute/Read): ")
            args(6) = Console.ReadLine()
            Console.WriteLine()
            Console.WriteLine()
        Catch ex As Exception


            ' If an error occurred, display the error message 
            Console.WriteLine()
            Console.WriteLine(ex.Message)

            Console.WriteLine()
        End Try

        Return args

    End Function

End Class