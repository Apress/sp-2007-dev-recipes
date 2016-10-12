Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.SharePoint

Module Module1

    Sub Main(ByVal args As String())
        ' Get handle to target site, web and list 
        Dim site As New SPSite("http://mgerow-moss-vpc")
        Dim web As SPWeb = site.AllWebs("EventHandlers")
        Dim list As SPList = web.Lists("UpdateFieldsList")

        For i As Integer = list.EventReceivers.Count - 1 To 0 Step -1
            ' Remove any pre-existing event receivers 
            list.EventReceivers(i).Delete()
        Next

        ' Add the new event receiver 
        Dim asmName As String = "UpdateFieldsEventHandler-VB, Version=1.0.0.0, Culture=neutral, PublicKeyToken=809b67cef68bed7f"
        Dim className As String = "UpdateFieldsEventHandler_VB.UpdateFieldsEventHandler"
        list.EventReceivers.Add(SPEventReceiverType.ItemAdded, asmName, className)
        list.EventReceivers.Add(SPEventReceiverType.ItemAdding, asmName, className)
        list.EventReceivers.Add(SPEventReceiverType.ItemUpdated, asmName, className)

    End Sub

End Module
