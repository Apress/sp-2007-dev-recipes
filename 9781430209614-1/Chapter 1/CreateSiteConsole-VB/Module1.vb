Imports Microsoft.SharePoint
Imports System

'-------------------------------'
' Project: CreateSiteConsole-VB '
'-------------------------------'
Module Module1

    Public Sub Main(ByVal args() As String)

        ' Steps 1-2: If no arguments have been passed, prompt user to enter them now
        If (args.Length = 0) Then
            args = New String((7) - 1) {}
            args = GetParams(args)
        End If

        ' Add the new site (web) using the provided arguments
        AddSite(args)

    End Sub

    Private Sub AddSite(ByVal args() As String)
        Try

            ' Get a handle to the site collection to which the
            ' new site will be added.
            Dim strSitePath As String = args(0)
            Dim site As SPSite = New SPSite(strSitePath)

            ' Add the new site (web)
            ' ----------------------
            ' Step 3: Add the site collection
            ' args(0) = Site collection url
            ' args(1) = Site (web) url
            ' args(2) = Title
            ' args(3) = Description
            ' args(4) = Template
            Dim web As SPWeb = site.AllWebs.Add(args(1), _
                args(2), _
                args(3), _
                CType(1033, UInteger), _
                args(4), _
                False, _
                False)

            ' Step 5: Confirm site (web) information
            Console.WriteLine()
            Console.WriteLine(("Site \'" _
                            + (args(0) + "\' successfuly created.")))
            Console.WriteLine()
            DisplayParams(args)

            'Remove objects from memory
            web.Dispose()
            site.Dispose()

        Catch ex As Exception

            ' Step 6: If error occurs, display parameters and 
            ' Error message
            Console.WriteLine()
            Console.WriteLine("** ERROR OCCURRED **")
            Console.WriteLine()
            Console.WriteLine(ex.Message)
            Console.WriteLine()
            DisplayParams(args)
            Console.WriteLine()

        End Try

    End Sub

    Private Sub DisplayParams(ByVal args() As String)

        Try

            ' Step 7: Display parameters to console
            Console.WriteLine("Site collection url: " + args(0))
            Console.WriteLine("Site (web): " + args(1))
            Console.WriteLine("Title: " + args(2))
            Console.WriteLine("Description: " + args(3))
            Console.WriteLine("Template: " + args(4))

        Catch ex As Exception

            ' If error occurred, display the error message
            Console.WriteLine()
            Console.WriteLine(ex.Message)
            Console.WriteLine()

        End Try

    End Sub

    Private Function GetParams(ByRef args() As String) As String()

        Try

            ' Step 2: Get parameters from user
            Console.WriteLine()
            Console.Write("Site collection url: ")
            args(0) = Console.ReadLine
            Console.Write("Site (web): ")
            args(1) = Console.ReadLine
            Console.Write("Title: ")
            args(2) = Console.ReadLine
            Console.Write("Description: ")
            args(3) = Console.ReadLine
            Console.Write("Template: ")
            args(4) = Console.ReadLine

        Catch ex As Exception

            ' If an error occurred, display the error message
            Console.WriteLine()
            Console.WriteLine(ex.Message)
            Console.WriteLine()

        End Try

        Return args

    End Function

End Module