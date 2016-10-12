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
Imports Microsoft.SharePoint.WebControls
Imports Microsoft.SharePoint.WebPartPages
Imports System.Xml

Partial Public Class _Default
    Inherits System.Web.UI.Page

    Private Shared Function AddContentEditorWebPart(ByVal strContent As String, _
        ByVal strTitle As String, _
        ByVal strSiteUrl As String, _
        ByVal strWebName As String, _
        ByVal strDocLibName As String, _
        ByVal strPage As String, _
        ByVal strZone As String, _
        ByVal numOrder As Integer, _
        ByVal pScope As System.Web.UI.WebControls.WebParts.PersonalizationScope) _
        As Microsoft.SharePoint.WebPartPages.ContentEditorWebPart

        Try
            ' Create an empty content editor web part. 
            Dim cewp As New Microsoft.SharePoint.WebPartPages.ContentEditorWebPart()

            ' Create an xml element object and transfer the content 
            'into the web part. 
            Dim xmlDoc As New XmlDocument()
            Dim xmlElem As System.Xml.XmlElement = xmlDoc.CreateElement("xmlElem")
            xmlElem.InnerText = strContent
            cewp.Content = xmlElem

            ' Call generic method to add the web part 
            cewp = AddWebPart(cewp, _
                strTitle, _
                strSiteUrl, _
                strWebName, _
                strDocLibName, _
                strPage, _
                strZone, _
                numOrder, _
                System.Web.UI.WebControls.WebParts.PersonalizationScope.[Shared])

            Return cewp

        Catch ex As Exception
            Throw New Exception("AddContentEditorWebPart() error: " + ex.Message)
        End Try

    End Function

    Private Shared Function AddWebPart(ByVal oWebPart As System.Web.UI.WebControls.WebParts.WebPart, _
        ByVal strTitle As String, _
        ByVal strSiteUrl As String, _
        ByVal strWebName As String, _
        ByVal strDocLibName As String, _
        ByVal strPage As String, _
        ByVal strZone As String, _
        ByVal numOrder As Integer, _
        ByVal pScope As System.Web.UI.WebControls.WebParts.PersonalizationScope) _
        As System.Web.UI.WebControls.WebParts.WebPart

        Try
            ' Get handles to site, web and page to which 
            ' web part will be added. 
            Dim site As New SPSite(strSiteUrl)
            Dim web As SPWeb = site.OpenWeb(strWebName)

            ' Enable update of page 
            web.AllowUnsafeUpdates = True
            Dim webParts As SPLimitedWebPartManager
            If (strDocLibName <> "") Then
                webParts = web.GetLimitedWebPartManager(strDocLibName + "/" + strPage, pScope)
            Else
                webParts = web.GetLimitedWebPartManager(strPage, pScope)
            End If

            ' If web part page is in a document library, 
            ' disable checkout requirement 
            ' for duration of update 
            Dim list As SPList = Nothing
            Dim origForceCheckoutValue As Boolean = False
            If (strDocLibName <> "") Then
                list = web.Lists(strDocLibName)
                origForceCheckoutValue = list.ForceCheckout
                list.ForceCheckout = False
                list.Update()
            End If

            ' Add the web part 
            oWebPart.Title = strTitle
            webParts.AddWebPart(oWebPart, strZone, numOrder)

            ' Save changes back to the SharePoint database 
            webParts.SaveChanges(oWebPart)
            web.Update()

            ' If necessary, restore ForceCheckout setting 
            If (strDocLibName <> "") Then
                list.ForceCheckout = origForceCheckoutValue
                list.Update()
            End If
            Return oWebPart

        Catch ex As Exception
            Throw New Exception(("AddWebPart() error: " + ex.Message))
        End Try

    End Function

    Protected Sub Finish_Click(ByVal sender As Object, ByVal e As EventArgs)

        ' Step 1: Get handle to web site being created 
        Dim web As SPWeb = SPControl.GetContextWeb(Context)
        web.AllowUnsafeUpdates = True

        ' Step 2: If requested, cleare out any existing 
        ' web parts 
        If cbRemoveExisting.Checked Then
            Dim webparts As SPLimitedWebPartManager = web.GetLimitedWebPartManager("default.aspx", PersonalizationScope.[Shared])
            For i As Integer = webparts.WebParts.Count - 1 To -1 + 1 Step -1
                webparts.DeleteWebPart(webparts.WebParts(i))
            Next
        End If

        ' Step 3: If requested, add an instance of a SharePoint 
        ' "ContentEditorWebPart", which is a descendent of 
        ' the generic .NET 2.0 WebPart class 
        If cbCEWP.Checked Then
            Dim oCEwp As New Microsoft.SharePoint.WebPartPages.ContentEditorWebPart()
            oCEwp = AddContentEditorWebPart("Hello World!", "Hello World web part", web.Site.Url.ToString(), web.ServerRelativeUrl.Substring(1), "", "Default.aspx", _
            "Right", 0, System.Web.UI.WebControls.WebParts.PersonalizationScope.[Shared])
        End If

        ' Step 4: If requested, add a PageViewer web part 
        If cbPVWP.Checked Then
            Dim oPVwp As New Microsoft.SharePoint.WebPartPages.PageViewerWebPart()
            oPVwp.SourceType = PathPattern.URL
            oPVwp.ContentLink = txtPVUrl.Text
            oPVwp.Height = "1000px"
            oPVwp = DirectCast(AddWebPart(oPVwp, "And here's my page viewer web part!", web.Site.Url.ToString(), web.ServerRelativeUrl.Substring(1), "", "Default.aspx", _
            "Left", 0, System.Web.UI.WebControls.WebParts.PersonalizationScope.[Shared]), Microsoft.SharePoint.WebPartPages.PageViewerWebPart)
        End If

        ' Step 5: Now take the user to the home page of the new 
        ' web site 
        Response.Redirect(web.ServerRelativeUrl)

    End Sub

    ' Determine whether Url textbox should 
    ' be enabled based on whether 
    ' option to add a Page Viewer web part 
    ' is checked 
    Protected Sub cbPVWP_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
        txtPVUrl.Enabled = cbPVWP.Checked
    End Sub

End Class