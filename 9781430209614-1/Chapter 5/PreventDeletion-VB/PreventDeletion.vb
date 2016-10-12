Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.SharePoint

Public Class PreventDeletion
    Inherits SPItemEventReceiver
    Private _properties As SPItemEventProperties

    Public Overloads Overrides Sub ItemDeleting(ByVal properties As SPItemEventProperties)
        MyBase.ItemDeleting(properties)

        _properties = properties
        SPSecurity.RunWithElevatedPrivileges(AddressOf deleteItem)

    End Sub

    Private Sub deleteItem()
        ' Step 1: get handle to list item that raised 
        ' the delete event 
        Dim item As SPListItem = _properties.ListItem

        ' Step 2: get object representing the user 
        ' attempting to delete the task 
        Dim web As SPWeb = _properties.ListItem.ParentList.ParentWeb
        Dim user As SPUser = web.Users(_properties.UserLoginName)

        ' Step 3: determine whether task can 
        ' be deleted 
        If item("% Complete") Is Nothing Then
            item("% Complete") = 0
        End If
        Dim ok2Delete As Boolean = _
            (item("Status").ToString() = "Not Started" And Single.Parse(item("% Complete").ToString()) = 0) _
            Or user.IsSiteAdmin _
            Or _properties.UserDisplayName = "System Account"

        ' Step 4: if task is in progress and user 
        ' requesting deletion isn't a site administrator 
        ' or the "System Account", prevent the 
        ' deletion. 
        If Not ok2Delete Then
            _properties.Cancel = True
            _properties.ErrorMessage = "Unable to delete task '" + item("Title").ToString() + _
                "'. Only site administrators may delete tasks on which work has already begun."
        End If

    End Sub

End Class
