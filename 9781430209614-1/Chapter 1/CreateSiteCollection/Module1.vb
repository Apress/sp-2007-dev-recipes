Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Administration

Module Module1

    Sub Main(ByVal args() As String)

        'Steps 1-2: If no arguments have been passed, prompt user to enter them now
        If args.Length = 0 Then
            ReDim args(7)
            args = GetParams(args)
        End If

        'Add the new site collection using the provided arguments
        AddSiteCollection(args)

    End Sub

    Private Function AddSiteCollection(ByVal args() As String) As Integer

        Try

            'Get a handle to the root site on server
            Dim arrSitePath As String() = args(0).Split("/")
            Dim strServerPath As String = arrSitePath(0) & "//" & arrSitePath(2)
            Dim site As New SPSite(strServerPath)

            'Get the list of site collections for the web application
            Dim siteCollection As SPSiteCollection = site.WebApplication.Sites

            'Step 3: Add the site collection
            'args(0) = Site url
            'args(1) = Title 
            'args(2) = Description
            'args(3) = Web template
            'args(4) = Owner login
            'args(5) = Owner name
            'args(6) = Owner email
            siteCollection.Add( _
                args(0), _
                args(1), _
                args(2), _
                1033, _
                args(3), _
                args(4), _
                args(5), _
                args(6))

            'Step 5: Confirm site collection information
            Console.WriteLine()
            Console.WriteLine("Site collection '" & args(0) & "' successfuly created.")
            Console.WriteLine()
            DisplayParams(args)

            ' Release memory used by SPSite object
            site.Dispose()

            Return 0

        Catch ex As Exception

            'Step 6: If error occurs, display parameters and error message
            Console.WriteLine()
            Console.WriteLine("** ERROR OCCURRED **")
            Console.WriteLine()
            Console.WriteLine(ex.Message)
            Console.WriteLine()
            DisplayParams(args)
            Console.WriteLine()

            Return -1

        End Try

        Console.WriteLine("")

    End Function

    Private Sub DisplayParams(ByVal args)
        Try

            'Step 7: Display parameters to console
            Console.WriteLine("Site url: " & args(0))
            Console.WriteLine("Title: " & args(1))
            Console.WriteLine("Description: " & args(2))
            Console.WriteLine("Template name: " & args(3))
            Console.WriteLine("Owner login: " & args(4))
            Console.WriteLine("Owner name: " & args(5))
            Console.WriteLine("Owner email: " & args(6))

        Catch ex As Exception

            'If error occurred, display the error message
            Console.WriteLine()
            Console.WriteLine(ex.Message)
            Console.WriteLine()

        End Try
    End Sub

    Private Function GetParams(ByRef args() As String) As String()
        Try

            'Step 2: Get parameters from user
            Console.WriteLine()
            Console.Write("Site url: ")
            args(0) = Console.ReadLine()
            Console.Write("Title: ")
            args(1) = Console.ReadLine()
            Console.Write("Description: ")
            args(2) = Console.ReadLine()
            Console.Write("Template name: ")
            args(3) = Console.ReadLine()
            Console.Write("Owner login: ")
            args(4) = Console.ReadLine()
            Console.Write("Owner name: ")
            args(5) = Console.ReadLine()
            Console.Write("Owner email: ")
            args(6) = Console.ReadLine()


        Catch ex As Exception

            'If an error occurred, display the error message
            Console.WriteLine()
            Console.WriteLine(ex.Message)
            Console.WriteLine()

        End Try

        Return args

    End Function

End Module
