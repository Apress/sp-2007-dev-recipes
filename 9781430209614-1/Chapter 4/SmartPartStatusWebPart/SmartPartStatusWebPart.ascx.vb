Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls

Partial Class SmartPartStatusWebPart
    Inherits System.Web.UI.UserControl

    'Define local variables
    Private _showImage As Boolean = True
    Private _statusListSiteUrl As String = "http://localhost"
    Private _statusListWeb As String = "spwp"
    Private _statusList As String = "Status"

    Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender

        'Only run this code if this is the first time the
        'user control has been displayed on the current
        'page since it was opened
        If Not IsPostBack Then
            HideAllImages()
            Select Case GetProfileStatus()
                Case "Green"
                    Me.imgGreen.Visible = True
                    Me.radGreen.Checked = True
                Case "Yellow"
                    Me.imgYellow.Visible = True
                    Me.radYellow.Checked = True
                Case "Red"
                    Me.imgRed.Visible = True
                    Me.radRed.Checked = True
                Case Else
            End Select
        End If

    End Sub

    Private Function GetProfileStatus() As String
        Try
            'Step 1: Create necessary objects
            Dim site As SPSite = New SPSite(_statusListSiteUrl)
            Dim web As SPWeb = site.AllWebs(_statusListWeb)
            Dim list As SPList = web.Lists(_statusList)

            'Step 2: Find the list item for the current user, 
            'and update its status
            For Each ListItem As SPListItem In list.Items
                Try
                    'Step 3: If match found, return the status
                    If ListItem("UserAlias").ToString.ToLower = _
                            Context.User.Identity.Name.ToLower Then
                        Return ListItem("Status")
                        Exit For
                    End If
                Catch ex As Exception
                    'No op
                End Try
            Next
        Catch ex As Exception
            'No op
        End Try

        'Step 4: If we got this far, no entry was found for the current user
        Return ""

    End Function

    Private Sub UpdateStatus()
        Try
            'Step 1: Get a handle to the list that we're using to store
            'user status information
            Dim site As SPSite = New SPSite(_statusListSiteUrl)
            Dim web As SPWeb = site.AllWebs(_statusListWeb)
            Dim list As SPList = web.Lists(_statusList)
            Dim listItem As SPListItem
            Dim boolFound As Boolean = False

            'Step 2: Find the list item for the current user, and update its status
            For Each listItem In list.Items
                Try
                    'Step 3: if user found, update their status
                    If listItem("UserAlias").ToString.ToLower = Context.User.Identity.Name.ToLower Then
                        listItem("Status") = GetUserControlStatus()
                        listItem.Update()
                        boolFound = True
                        Exit For
                    End If
                Catch ex As Exception
                End Try
            Next

            'Step 4: If an entry for the current user wasn't found in the list
            'add one now.
            If Not boolFound Then
                listItem = list.Items.Add()
                listItem("UserAlias") = Context.User.Identity.Name
                listItem("Status") = GetUserControlStatus()
                listItem.Update()
            End If

        Catch ex As Exception

            Dim lbl As New Label
            lbl.Text = "<font color='red'>" & ex.Message & "</font><br/>"
            Me.Controls.Add(lbl)

        End Try

    End Sub

    'Get the currently selected status from
    'the user control UI
    Private Function GetUserControlStatus() As String
        If radRed.Checked Then
            Return "Red"
        ElseIf radYellow.Checked Then
            Return "Yellow"
        Else
            Return "Green"
        End If
    End Function

    'Helper function to make sure all images are
    'hidden prior to displaying the selected one
    Public Sub HideAllImages()
        Me.imgGreen.Visible = False
        Me.imgYellow.Visible = False
        Me.imgRed.Visible = False
    End Sub

    'The following event handlers process button clicks to
    'display the image corresponding to the selected status
    Public Sub radGreen_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles radGreen.CheckedChanged
        HideAllImages()
        If radGreen.Checked Then
            If _showImage Then
                imgGreen.Visible = True
            End If
        End If
        UpdateStatus()
    End Sub
    Public Sub radYellow_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles radYellow.CheckedChanged
        HideAllImages()
        If radYellow.Checked Then
            If _showImage Then
                imgYellow.Visible = True
            End If
        End If
        UpdateStatus()
    End Sub
    Public Sub radRed_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles radRed.CheckedChanged
        HideAllImages()
        If radRed.Checked Then
            If _showImage Then
                imgRed.Visible = True
            End If
        End If
        UpdateStatus()
    End Sub

End Class
