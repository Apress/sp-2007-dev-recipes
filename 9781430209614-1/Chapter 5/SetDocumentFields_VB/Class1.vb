Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.SharePoint

Public Class SetDocumentFields
    Inherits SPItemEventReceiver

    ' Name of hidden item that contains default properties 
    Const DEFAULT_FILE_TITLE As String = "[defaults]"

    ' local varialble to contain data passed in from SharePoint 
    Private _properties As SPItemEventProperties

    ' Method called AFTER a new item has been added 
    Public Overloads Overrides Sub ItemAdded(ByVal properties As SPItemEventProperties)
        MyBase.ItemAdded(properties)
        _properties = properties

        ' Run under system account 
        SPSecurity.RunWithElevatedPrivileges(AddressOf setFields)
    End Sub

    Private Sub setFields()
        ' Step 1: Get a handle to the item that raised the event 
        Dim item As SPListItem = _properties.ListItem

        ' Step 2: Get a handle to folder containing the 
        ' document just uploaded 
        Dim folder As SPFolder = Nothing
        Try
            If item.File.ParentFolder IsNot Nothing Then
                folder = item.File.ParentFolder
            Else
                folder = item.ParentList.RootFolder
            End If
            ' No op 
        Catch ex As Exception
        End Try

        ' Step 3: Assuming a folder was found (which 
        ' should be in all cases, find the associated 
        ' [defaults] document. 
        If folder IsNot Nothing Then

            Dim files As SPFileCollection = folder.Files
            Dim client As String = ""
            Dim matter As String = ""

            ' Step 4: Find the document containing defaults, and 
            ' use its "Client" and "Matter" values for 
            ' new document 
            For Each file As SPFile In files
                If file.Title.ToLower() = DEFAULT_FILE_TITLE Then
                    client = file.Item("Client").ToString()
                    matter = file.Item("Matter").ToString()
                    Exit For
                End If
            Next
            item("Client") = client
            item("Matter") = matter

            ' Step 5: Save changes without updating the 
            ' MODIFIED, MODIFIED BY fields 
            item.SystemUpdate(False)

        End If
    End Sub
End Class