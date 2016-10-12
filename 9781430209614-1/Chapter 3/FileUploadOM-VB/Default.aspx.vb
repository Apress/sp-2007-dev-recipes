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
Imports System.IO

Partial Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub cmdUploadFile_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUploadFile.Click
        Try
            lblErrorMsg.Visible = False

            ' Step 1: Get handle to site collection, web site, list 
            Dim site As New SPSite(txtSiteUrl.Text)
            Dim web As SPWeb = site.AllWebs(txtWebName.Text)
            Dim dl As SPList = web.Lists(txtDocLibName.Text)
            Dim file As SPFile

            web.AllowUnsafeUpdates = True
            web.Lists.IncludeRootFolder = True

            ' Step 2: Make sure user has selected a file 
            If FileUpload1.PostedFile.FileName <> "" Then

                ' Step 3: load the content of the file into a byte array 
                Dim fStream As Stream
                Dim contents As Byte() = New Byte(FileUpload1.PostedFile.InputStream.Length - 1) {}

                fStream = FileUpload1.PostedFile.InputStream
                fStream.Read(contents, 0, CInt(fStream.Length))
                fStream.Close()

                ' Step 4: upload the file to SharePoint doclib 
                file = web.Files.Add(web.Url.ToString() + "/" + dl.Title.ToString() + "/" + FileUpload1.FileName, contents)

            Else
                lblErrorMsg.Text = "Please select a file to upload"
                lblErrorMsg.Visible = True
            End If

        Catch ex As Exception
            lblErrorMsg.Text = ex.Message
            lblErrorMsg.Visible = True
        End Try

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

End Class