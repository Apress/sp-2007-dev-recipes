Imports System.Web
Imports Microsoft.SharePoint

Public Class SendEmail
    Inherits SPItemEventReceiver

    Private _properties As SPItemEventProperties

    Public Overrides Sub ItemUpdated(ByVal properties As Microsoft.SharePoint.SPItemEventProperties)
        MyBase.ItemUpdated(properties)

        _properties = properties
        SPSecurity.RunWithElevatedPrivileges(AddressOf emailIfComplete)

    End Sub

    Private Sub emailIfComplete()

        'Step 1: Get a handle to the task that
        '   raised the update event
        Dim item As SPListItem = _properties.ListItem

        'Step 2: Determine if email should be sent based
        '   on current status of task.  If so...
        If item("Status") = "Completed" Then
            SendEmail( _
                "mgerow@fenwick.com", _
                "tasklist@fenwick.com", _
                "Task '" & item("Title").ToString() & "' is Complete", _
                "The task was marked complete by '" & _properties.UserDisplayName.ToString() & "'.", _
                "SVCTA2.firm.fenwick.llp")
        End If

    End Sub

    Private Sub SendEmail( _
        ByVal Recipient As String, _
        ByVal Sender As String, _
        ByVal Subject As String, _
        ByVal Message As String, _
        ByVal Server As String _
        )

        'Step 3: Create message and SMTP client objects
        Dim msg As System.Net.Mail.MailMessage
        Dim smtpClient As System.Net.Mail.SmtpClient = New System.Net.Mail.SmtpClient(Server)

        'Step 4: Construct and send the message
        msg = New System.Net.Mail.MailMessage(Sender, Recipient, Subject, Message)
        msg.IsBodyHtml = True
        smtpClient.Send(msg)

    End Sub

End Class
