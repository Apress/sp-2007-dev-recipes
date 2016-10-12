Imports Microsoft.SharePoint

Module Module1

    Sub Main(ByVal args() As String)

        'Step 1: If no arguments passed in, prompt for them now
        If (args.Length = 0) Then
            args = New String(7) {}
            args = GetParams(args)
        End If

        Try

            'Step 2: Get handle to specifie site collection and web site
            Dim site As New SPSite(args(0))
            Dim web As SPWeb = site.AllWebs(args(1))

            'Step 3: Add the user to the specified site group
            web.SiteGroups(args(6)).AddUser(args(2), args(3), args(4), args(5))

            'Step 4-5: Display error message
            Console.WriteLine("User '" & args(2) & "' has been successfully added to site group '" & args(6) & "'")

        Catch ex As Exception

            'Step 6: If error occurred, display error message
            Console.WriteLine(ex.Message)

        End Try

        Console.WriteLine()
        Console.WriteLine("Press any key to continue...")
        Console.Read()

    End Sub

    Private Function GetParams(ByRef args() As String) As String()
        Try

            ' Step 2: Get parameters from user
            Console.WriteLine()
            Console.Write("Site collection url: ")
            args(0) = Console.ReadLine
            Console.Write("Site (web): ")
            args(1) = Console.ReadLine
            Console.Write("User login: ")
            args(2) = Console.ReadLine
            Console.Write("Email address: ")
            args(3) = Console.ReadLine
            Console.Write("User name: ")
            args(4) = Console.ReadLine
            Console.Write("Notes: ")
            args(5) = Console.ReadLine
            Console.Write("Site Group to add user to: ")
            args(6) = Console.ReadLine
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

End Module
