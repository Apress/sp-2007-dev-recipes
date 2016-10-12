Imports Microsoft.SharePoint.Administration
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls

Partial Class _Default
    Inherits System.Web.UI.Page

    Protected Sub LinkButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LinkButton1.Click

        Try

            'Get list of templates for the selected web application
            Dim site As New SPSite(txtSiteCollPath.Text)
            Dim templateCollection As SPWebTemplateCollection = site.GetWebTemplates(1033)
            Dim template As SPWebTemplate
            Dim listItem As ListItem

            ddlTemplate.Items.Clear()

            For Each template In templateCollection
                listItem = New ListItem(template.Title, template.Name)
                ddlTemplate.Items.Add(listItem)
            Next
            ddlTemplate.Enabled = True

            lblMessage.Text = ""

        Catch ex As Exception

            lblMessage.Text = ex.Message

        End Try

    End Sub

    Protected Sub cmdCreateNewSiteCollection_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCreateNewSiteCollection.Click

        'Dim strExePath As String = """C:\SharePoint Recipe Solutions\Chapter 1\CreateSiteCollection\bin\Debug\CreateSiteCollection.exe"""
        'strExePath += " """ & txtSiteCollPath.Text & "/" & txtSiteName.Text & """"
        'strExePath += " """ & txtTitle.Text & """"
        'strExePath += " """ & txtDescription.Text & """"
        'strExePath += " """ & ddlTemplate.SelectedValue & """"
        'strExePath += " """ & txtOwnerLogin.Text & """"
        'strExePath += " """ & txtOwnerName.Text & """"
        'strExePath += " """ & txtOwnerEmail.Text & """"

        'Dim intRetVal = _
        'Microsoft.VisualBasic.Shell( _
        '    strExePath, _
        '    AppWinStyle.NormalFocus, _
        '    True, _
        '    -1)

        SPSecurity.RunWithElevatedPrivileges(AddressOf AddSiteCollection)

    End Sub

    Private Sub AddSiteCollection()

        Try

            Dim site As New SPSite(txtSiteCollPath.Text)
            Dim siteCollection As SPSiteCollection = site.WebApplication.Sites
            'siteCollection.Add( _
            '    txtSiteCollPath.Text & "/" & txtSiteName.Text, _
            '    txtTitle.Text, _
            '    txtDescription.Text, _
            '    1033, _
            '    ddlTemplate.SelectedValue, _
            '    txtOwnerLogin.Text, _
            '    txtOwnerName.Text, _
            '    txtOwnerEmail.Text)
            siteCollection.Add( _
                txtSiteCollPath.Text & "/" & txtSiteName.Text, _
                txtOwnerLogin.Text, _
                txtOwnerEmail.Text)

        Catch ex As Exception

            lblMessage.Text = ex.Message

        End Try

    End Sub
End Class
