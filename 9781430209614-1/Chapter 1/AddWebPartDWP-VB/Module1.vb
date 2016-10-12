Imports System.Xml
Imports System.Web
Imports System.Web.UI.WebControls
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls
Imports Microsoft.SharePoint.WebPartPages

Module Module1

    Sub Main(ByVal args() As String)

        If args.Length < 6 Then
            GetArgs(args)
        End If

        Console.WriteLine(AddWebPartDWP(args(0), args(1), args(2), args(3), WebParts.PersonalizationScope.Shared, args(4), args(5)))
        Console.WriteLine()
        Console.WriteLine("Press any key to continue...")
        Console.Read()

    End Sub

    Private Sub GetArgs(ByRef args() As String)
        ReDim args(5)
        Console.WriteLine()
        Console.Write("Site Url: ")
        args(0) = Console.ReadLine()
        Console.Write("Web Name: ")
        args(1) = Console.ReadLine
        Console.Write("Doclib containing page (leave blank for none): ")
        args(2) = Console.ReadLine
        Console.Write("Page Name: ")
        args(3) = Console.ReadLine
        Console.Write("Path to DWP: ")
        args(4) = Console.ReadLine
        Console.Write("Zone: ")
        args(5) = Console.ReadLine
        Console.WriteLine()
    End Sub

    Public Function AddWebPartDWP( _
        ByVal strSiteUrl As String, _
        ByVal strWebName As String, _
        ByVal strDocLibName As String, _
        ByVal strPage As String, _
        ByVal pScope As System.Web.UI.WebControls.WebParts.PersonalizationScope, _
        ByVal strDWPPath As String, _
        ByVal strZone As String _
        ) As String

        Try

            'Get handle to site and web to be edited
            Dim site As New SPSite(strSiteUrl)
            Dim web As SPWeb = site.OpenWeb(strWebName)
            Dim wp As WebPart

            'Step 1: Get handle to web part page to contain new web part
            Dim webParts As SPLimitedWebPartManager
            If strDocLibName > "" Then
                webParts = web.GetLimitedWebPartManager(strDocLibName & "/" & strPage, pScope)
            Else
                webParts = web.GetLimitedWebPartManager(strPage, pScope)
            End If

            'Step 2: get handle to .DWP file and import definition
            Dim reader As XmlReader = XmlReader.Create(strDWPPath)
            wp = webParts.ImportWebPart(reader, "")

            'Step 3: allow web site to be edited
            web.AllowUnsafeUpdates = True

            'Step 4-5: If web part page is in a document library, 
            'disable checkout requirement for duration of update
            Dim list As SPList
            Dim origForceCheckoutValue As Boolean
            If strDocLibName > "" Then
                list = web.Lists(strDocLibName)
                origForceCheckoutValue = list.ForceCheckout
                list.ForceCheckout = False
                list.Update()
            End If

            'Step 6: Add the web part
            webParts.AddWebPart(wp, strZone, 0)

            'Step 7: Save changes back to SharePoint database
            webParts.SaveChanges(wp)
            web.Update()

            'Step 8-9: If necessary, restore ForceCheckout setting
            If strDocLibName > "" Then
                list.ForceCheckout = origForceCheckoutValue
                list.Update()
            End If

            Return "Successfuly added '" & strDWPPath & "'"

        Catch ex As Exception

            Return ex.Message

        End Try

    End Function

End Module
