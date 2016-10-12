Imports Microsoft.SharePoint

Module Module1

    Sub Main()

        Dim site As New SPSite("http://mgerow-moss-vpc")
        Dim web As SPWeb = site.AllWebs("EventHandlers")
        Dim tasks As SPList = web.Lists("Tasks")

        Dim i As Integer
        For i = tasks.EventReceivers.Count - 1 To 0 Step -1
            tasks.EventReceivers(i).Delete()
        Next

        'Add the variables event handler
        Dim asmName As String = "SendEmail-VB, Version=1.0.0.0, Culture=neutral, PublicKeyToken=4eb0175192d1b499"
        Dim className As String = "SendEmail_VB.SendEmail"
        tasks.EventReceivers.Add(SPEventReceiverType.ItemUpdated, asmName, className)
        tasks.Update()

    End Sub

End Module
