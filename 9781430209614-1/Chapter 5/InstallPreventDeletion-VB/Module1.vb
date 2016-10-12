Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.SharePoint

Module Module1

    Sub Main(ByVal args As String())
        ' Get handle to target site, web and list 
        Dim site As New SPSite("http://mgerow-moss-vpc")
        Dim web As SPWeb = site.AllWebs("EventHandlers")
        Dim list As SPList = web.Lists("Tasks")

        For i As Integer = list.EventReceivers.Count - 1 To -1 + 1 Step -1
            ' Remove any pre-existing event receivers 
            list.EventReceivers(i).Delete()
        Next

        ' Add the new event receiver 
        Dim asmName As String = "PreventDeletion-VB, Version=1.0.0.0, Culture=neutral, PublicKeyToken=551b0daa60f2e217"
        Dim className As String = "PreventDeletion_VB.PreventDeletion"
        list.EventReceivers.Add(SPEventReceiverType.ItemDeleting, asmName, className)

    End Sub

End Module