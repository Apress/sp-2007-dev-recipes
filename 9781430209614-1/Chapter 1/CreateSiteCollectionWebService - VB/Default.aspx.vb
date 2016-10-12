Partial Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try

            If Not IsPostBack Then

                'Step 1: Get list of templates for the selected web application
                Dim objSites As New SitesService.Sites
                objSites.Credentials = System.Net.CredentialCache.DefaultCredentials
                Dim arrTemplates() As SitesService.Template
                Dim templateCount As Integer = objSites.GetSiteTemplates(1033, arrTemplates)
                Dim i As Integer
                Dim listItem As ListItem

                ddlTemplate.Items.Clear()

                For i = 0 To arrTemplates.Length - 1

                    'Don't include hidden templates, which are not intended for interactive use
                    If Not arrTemplates(i).IsHidden Then
                        listItem = New ListItem(arrTemplates(i).Title, arrTemplates(i).Name)
                        ddlTemplate.Items.Add(listItem)
                    End If

                Next
                ddlTemplate.Enabled = True

                lblMessage.Text = ""

            End If

        Catch ex As Exception

            lblMessage.Text = ex.Message

        End Try

    End Sub

    Protected Sub cmdCreateNewSiteCollection_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCreateNewSiteCollection.Click

        Try

            'Step 2: Make sure all necessary data is provided
            If txtSiteCollPath.Text > "" _
                And txtSiteName.Text > "" _
                And txtTitle.Text > "" _
                And txtDescription.Text > "" _
                And ddlTemplate.SelectedValue > "" _
                And txtOwnerLogin.Text > "" _
                And txtOwnerName.Text > "" _
                And txtOwnerEmail.Text > "" Then

                'Step 4: Add new site collection 
                Dim objAdmin As New AdminService.Admin
                objAdmin.Credentials = System.Net.CredentialCache.DefaultCredentials
                objAdmin.CreateSite( _
                    txtSiteCollPath.Text & "/" & txtSiteName.Text, _
                    txtTitle.Text, _
                    txtDescription.Text, _
                    1033, _
                    ddlTemplate.SelectedValue, _
                    txtOwnerLogin.Text, _
                    txtOwnerName.Text, _
                    txtOwnerEmail.Text, _
                    "", "")

                'Step 6: Display success message
                lblMessage.Text = "Successfuly added new site"
                lblMessage.Visible = True

            Else

                'Step 3: Prompt user to enter all data
                lblMessage.Text = "Please fill in all fields"
                lblMessage.Visible = True

            End If

        Catch ex As Exception

            'Step 7: Display error message
            lblMessage.Text = ex.Message
            lblMessage.Visible = True

        End Try

    End Sub

End Class
