Imports Microsoft.SharePoint.Administration
Imports Microsoft.SharePoint

Public Class CreateSiteCollection

    Public Function AddSiteCollection(ByVal args() As String) As Integer

        Try

            'Get a handle to the root site
            Dim site As New SPSite("http://localhost")

            'Get the list of site collections for the web application
            Dim siteCollection As SPSiteCollection = site.WebApplication.Sites

            'Add the site collection
            'args(0) = Site url
            'args(1) = Title 
            'args(2) = Description
            'args(3) = Web template
            'args(4) = Owner login
            'args(5) = Owner name
            'args(6) = Owner email

            site = siteCollection.Add( _
                args(0), _
                args(1), _
                args(2), _
                1033, _
                args(3), _
                args(4), _
                args(5), _
                args(6))

            site.Dispose()

            Return 0

        Catch ex As Exception

            'If error occurs, return error code
            Return -1

        End Try

        Console.WriteLine("")

    End Function

End Class
