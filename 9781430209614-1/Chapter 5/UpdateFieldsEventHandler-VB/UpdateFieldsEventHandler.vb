Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls

Public Class UpdateFieldsEventHandler
    Inherits SPItemEventReceiver

    ' Local veriable used to pass event properties to 
    ' setFields() method. 
    Private _properties As SPItemEventProperties

    Const errMsg As String = "Rejected because order $Amt too large, please break into orders of less than $100,000."

    ' The ItemAdding method is called BEFORE the item is 
    ' written to the SharePoint content database. If 
    ' this event is cancelled the item will be discarded. 
    Public Overrides Sub ItemAdding(ByVal properties As SPItemEventProperties)
        MyBase.ItemAdding(properties)
        _properties = properties

        ' Run as system account to avoid any permission issues 
        Microsoft.SharePoint.SPSecurity.RunWithElevatedPrivileges(AddressOf validateAddUpdate)
    End Sub

    ' The ItemAdding method is called AFTER the item has been 
    ' written to the content database. Place code or method 
    ' calls here that will update fields or take action based 
    ' on data saved. 
    Public Overrides Sub ItemAdded(ByVal properties As SPItemEventProperties)
        MyBase.ItemAdded(properties)
        _properties = properties

        ' Run as system account to avoid any permission issues 
        Microsoft.SharePoint.SPSecurity.RunWithElevatedPrivileges(AddressOf setFields)
    End Sub

    ' The ItemUpdated method is called after changes are written 
    ' to the SharePoint content database. Place code or method 
    ' calls here to update fields or take action based on values 
    ' written. 
    Public Overrides Sub ItemUpdated(ByVal properties As SPItemEventProperties)
        MyBase.ItemUpdated(properties)
        _properties = properties

        ' Run as system account to avoid any permission issues 
        Microsoft.SharePoint.SPSecurity.RunWithElevatedPrivileges(AddressOf setFields)
    End Sub

    ' Validation has been passed, so apply business 
    ' rules to update the approval fields. 
    Private Sub setFields()
        Try
            ' Step 1: get a handle to the list item 
            ' that raised this event 
            Dim li As SPListItem = _properties.ListItem

            ' Step 2: Calculate the extended price 
            Dim extPrice As Single = Single.Parse(li("Price").ToString()) * Single.Parse(li("Quantity").ToString())

            ' Step 3: Determine if approval is required 
            Dim approvalRequired As Boolean = (extPrice > 5000)
            li("Extended Price") = extPrice
            li("Approval Required?") = approvalRequired

            ' Step 4: If approval is required, assign to approver 
            If approvalRequired Then
                ' Step 5: Assigne default value to note field 
                li("Notes") = "Approver assigned at " + DateTime.Now.ToString()

                ' Step 6: This is where the business logic gets applied. 
                ' Of course your business logic will likely be more 
                ' complex, but the process is the same 
                If extPrice < 10000 Then
                    li("Approver") = "Dept Mgr"
                Else
                    If extPrice < 25000 Then
                        li("Approver") = "CFO"
                    Else
                        If extPrice < 100000 Then
                            li("Approver") = "President/CEO"
                        Else
                            li("Notes") = errMsg
                            li("Approver") = ""
                            li("Approval Required?") = False
                            li("Quantity") = 0
                            li("Extended Price") = 0
                        End If
                    End If
                End If
            End If

            ' Step 7: update item, but don't reset 
            ' the MODIFIED or MODIFIED BY fields. 
            li.SystemUpdate()
            ' Handle error 
        Catch ex As Exception
        End Try
    End Sub

    ' This method checks to see if user has entered 
    ' Price and/or Quantity that results in an 
    ' Extended Amount in excess of $100,000 
    Private Sub validateAddUpdate()
        Dim extPrice As Single = Single.Parse(_properties.AfterProperties("Price").ToString()) * Single.Parse(_properties.AfterProperties("Quantity").ToString())

        ' Check to see if new item exceed extended price limit of $100,000 
        If extPrice > 99999 AndAlso _properties.EventType = SPEventReceiverType.ItemAdding Then
            _properties.Cancel = True
            _properties.ErrorMessage = errMsg
        End If
    End Sub

End Class
