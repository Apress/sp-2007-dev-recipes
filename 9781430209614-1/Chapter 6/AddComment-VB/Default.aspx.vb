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

Partial Public Class _Default
    Inherits System.Web.UI.Page

    Const COMMENTS_SITE_URL As String = "http://mgerow-moss-vpc"
    Const COMMENTS_WEB_NAME As String = "DOCS"
    Const COMMENTS_LIST As String = "DocComments"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        Try
            ' Step 2: Get parse querystring for parameters. 
            ' We are assuming that this page is hosted in 
            ' a page viewer web part on a web part page. 
            ' So get the parameters passed to the web-part 
            ' page contining this page in a PageViewer 
            Dim querystring As String() = Server.UrlDecode(Request.UrlReferrer.Query).ToLower().Replace("?", "").Split("&"c)
            For i As Integer = 0 To querystring.Length - 1

                ' Step 3: Display querystring parameters 
                If querystring(i).IndexOf("itemid=") <> -1 Then
                    lblItemId.Text = querystring(i).Split("="c)(1)
                End If
                If querystring(i).IndexOf("itemurl=") <> -1 Then
                    hlItemUrl.NavigateUrl = querystring(i).Split("="c)(1)
                    hlItemUrl.Text = querystring(i).Split("="c)(1)
                End If
                If querystring(i).IndexOf("listid=") <> -1 Then
                    lblListId.Text = querystring(i).Split("="c)(1)
                End If
                If querystring(i).IndexOf("siteurl=") <> -1 Then
                    hlSiteUrl.NavigateUrl = querystring(i).Split("="c)(1)
                    hlSiteUrl.Text = querystring(i).Split("="c)(1)
                End If
            Next

            ' Step 4: Get the list name and url from its GUID 
            Dim site As New SPSite(hlSiteUrl.NavigateUrl)

            'Dim web As SPWeb = site.OpenWeb(hlSiteUrl.NavigateUrl.ToLower().Replace(site.Url, ""))
            Dim webUrl As String = hlSiteUrl.NavigateUrl.ToLower().Replace(site.Url, "")
            If webUrl.IndexOf("/") = 0 Then
                webUrl = webUrl.Substring(1)
            End If
            Dim web As SPWeb = site.OpenWeb(webUrl)

            Dim guid As New Guid(lblListId.Text)
            Dim origList As SPList = web.Lists(guid)
            lblListName.Text = origList.Title
            hlListUrl.NavigateUrl = web.Url + "/" + origList.RootFolder.Url
            hlListUrl.Text = web.Url + "/" + origList.RootFolder.Url

            ' Step 5: Display existing comments for this document 
            Dim siteComments As New SPSite(COMMENTS_SITE_URL)
            Dim webComments As SPWeb = siteComments.OpenWeb(COMMENTS_WEB_NAME)
            Dim docComments As SPList = webComments.Lists(COMMENTS_LIST)
            Dim dtComments As DataTable = docComments.Items.GetDataTable()
            dtComments.TableName = "Comments"
            Dim dvComments As New DataView(dtComments, "ItemUrl='" + hlItemUrl.NavigateUrl + "'", "Created DESC", DataViewRowState.CurrentRows)
            GridView1.DataSource = dvComments
            GridView1.DataBind()
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub cmdSave_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Get handle to web 
        Dim siteComments As New SPSite(COMMENTS_SITE_URL)
        Dim webComments As SPWeb = siteComments.OpenWeb(COMMENTS_WEB_NAME)
        webComments.AllowUnsafeUpdates = True

        ' Step 6: Write new comment to DOCCOMMENTS list 
        Dim docComments As SPList = webComments.Lists(COMMENTS_LIST)
        Dim item As SPListItem = docComments.Items.Add()
        item("ItemId") = lblItemId.Text
        item("ItemUrl") = hlItemUrl.NavigateUrl
        item("ListId") = lblListId.Text
        item("SiteUrl") = hlSiteUrl.NavigateUrl
        item("Comment") = txtComments.Text
        item.Update()

        ' Step 7: Return user to list 
        returnToList()
    End Sub

    Protected Sub cmdCancel_Click(ByVal sender As Object, ByVal e As EventArgs)
        returnToList()
    End Sub

    Private Sub returnToList()
        ' Because this page is running in a page viewer 
        ' (i.e. in an <IFRAME>), a redirect statement would 
        ' display the target url in the frame, whereas we 
        ' want the target page displayed at the top-level. 
        ' To accomplish this we'll insert a bit of JavaScript 
        ' to perform a window.open(). 
        Response.Write("<script>window.open('" + hlListUrl.NavigateUrl + "','_top');</script>")
    End Sub

End Class