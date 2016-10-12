Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls
Imports Microsoft.SharePoint.WebPartPages
Imports System.Web
Imports System.Web.UI.WebControls
Imports System.Xml

Module Module1

    Sub Main()

        'Example 1:
        'Add an instance of a SharePoint "ContentEditorWebPart", which
        'is a descendent of the generic .NET 2.0 WebPart class
        Dim oCEwp As New Microsoft.SharePoint.WebPartPages.ContentEditorWebPart
        oCEwp = AddContentEditorWebPart( _
            "Hello World!", _
            "Hello World web part", _
            "http://localhost", _
            "", _
            "Pages", _
            "Default.aspx", _
            "Right", _
            0, _
            WebParts.PersonalizationScope.Shared _
            )

        'Example 2:
        'Add a PageViewer web part
        Dim oPVwp As New Microsoft.SharePoint.WebPartPages.PageViewerWebPart
        oPVwp.SourceType = PathPattern.URL
        oPVwp.ContentLink = "http://www.fenwick.com"
        oPVwp = AddWebPart( _
            oPVwp, _
            "Hello World web part", _
            "http://localhost", _
            "", _
            "Pages", _
            "Default.aspx", _
             "Top", _
            0, _
            WebParts.PersonalizationScope.Shared _
            )

    End Sub

    Private Function AddContentEditorWebPart( _
            ByVal strContent As String, _
            ByVal strTitle As String, _
            ByVal strSiteUrl As String, _
            ByVal strWebName As String, _
            ByVal strDocLibName As String, _
            ByVal strPage As String, _
            ByVal strZone As String, _
            ByVal numOrder As Integer, _
            ByVal pScope As System.Web.UI.WebControls.WebParts.PersonalizationScope _
        ) As Microsoft.SharePoint.WebPartPages.ContentEditorWebPart

        Try
            'Create an empty content editor web part. 
            Dim cewp As New Microsoft.SharePoint.WebPartPages.ContentEditorWebPart
            'Create an xml element object and transfer the content into the web part. 
            Dim xmlDoc As New XmlDocument
            Dim xmlElem As System.Xml.XmlElement = xmlDoc.CreateElement("xmlElem")
            xmlElem.InnerText = strContent
            cewp.Content = xmlElem

            'Call generic method to add the web part 
            cewp = AddWebPart( _
                cewp, _
                strTitle, _
                strSiteUrl, _
                strWebName, _
                strDocLibName, _
                strPage, _
                strZone, _
                numOrder, _
                UI.WebControls.WebParts.PersonalizationScope.Shared _
                )
            Return cewp
        Catch ex As Exception
            Throw New Exception("AddContentEditorWebPart() error: " & ex.Message)
        End Try

    End Function

    Private Function AddWebPart( _
            ByVal oWebPart As Web.UI.WebControls.WebParts.WebPart, _
            ByVal strTitle As String, _
            ByVal strSiteUrl As String, _
            ByVal strWebName As String, _
            ByVal strDocLibName As String, _
            ByVal strPage As String, _
            ByVal strZone As String, _
            ByVal numOrder As Integer, _
            ByVal pScope As System.Web.UI.WebControls.WebParts.PersonalizationScope _
        ) As Web.UI.WebControls.WebParts.WebPart

        Try

            'Step 1: Get handles to site, web and page to which web part will be added. 
            Dim site As New SPSite(strSiteUrl)
            Dim web As SPWeb = site.OpenWeb(strWebName)

            'Step 2: Enable update of page 
            web.AllowUnsafeUpdates = True

            Dim webParts As SPLimitedWebPartManager
            If strDocLibName > "" Then
                webParts = web.GetLimitedWebPartManager(strDocLibName & "/" & strPage, pScope)
            Else
                webParts = web.GetLimitedWebPartManager(strPage, pScope)
            End If

            'If web part page is in a document library, disable checkout requirement 
            'for duration of update
            Dim list As SPList
            Dim origForceCheckoutValue As Boolean
            If strDocLibName > "" Then
                list = web.Lists(strDocLibName)
                origForceCheckoutValue = list.ForceCheckout
                list.ForceCheckout = False
                list.Update()
            End If

            'Add the web part 
            oWebPart.Title = strTitle
            webParts.AddWebPart(oWebPart, strZone, numOrder)

            'Save changes back to the SharePoint database 
            webParts.SaveChanges(oWebPart)
            web.Update()

            'If necessary, restore ForceCheckout setting
            If strDocLibName > "" Then
                list.ForceCheckout = origForceCheckoutValue
                list.Update()
            End If

            Return oWebPart

        Catch ex As Exception
            Throw New Exception("AddWebPart() error: " & ex.Message)
        End Try
    End Function

End Module

