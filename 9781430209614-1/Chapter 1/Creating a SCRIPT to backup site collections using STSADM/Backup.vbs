' --------------------------------------------------------------
' Purpose: 	Backup all SharePoint site collections on server
' By:		Mark Gerow
' Date:		1/3/08
' --------------------------------------------------------------
Option Explicit

' Set the path to the STSADM utility
Const STSADM_PATH =   _
 "C:\Program Files\Common Files\Microsoft Shared\web server extensions\12\BIN\stsadm"

' Set the path to where you want the backups made 
Const BACKUP_PATH = "C:\SharePoint_Backups\"
 
' Define needed variables
Dim objFso, objFolder, objFiles, objFile, objShell
Dim objExec, strResult, objXml, objSc, objUrl
Dim strUrl, strFileName, strCmd

' Step 1: OPTIONAL: Delete any pre-existing backups
Set objFso = CreateObject("Scripting.FileSystemObject")
Set objFolder = objFso.GetFolder(BACKUP_PATH)
Set objFiles = objFolder.Files
For Each objFile in objFiles
  objFile.Delete(True)
Next

' Step 2: Retrieve all site collections in XML format.
Set objShell = CreateObject("WScript.Shell")
Set objExec = objShell.Exec(STSADM_PATH & " -O ENUMSITES -URL http://localhost/")
strResult = objExec.StdOut.ReadAll

' Load XML in DOM document so it can be processed.
Set objXml = CreateObject("MSXML2.DOMDocument")
objXml.LoadXML(strResult) 

' Step 3: Loop through each site collection and call stsadm.exe to make a backup.
For Each objSc in objXml.DocumentElement.ChildNodes
    strUrl = objSc.Attributes.GetNamedItem("Url").Text
    strFileName = BACKUP_PATH & _
        Replace(Replace(strUrl,"/","_"),":","") & ".bak"
    strCmd = STSADM_PATH & " -O BACKUP -URL """ & _
        strUrl + """ -FILENAME """ + strFileName + """"
    
	' For testing, display pop-up for each collection backed up
	WScript.Echo "Backing up site collection " & _
		strUrl & " to file " & _
		strFileName & " using the following command " & _
		strCmd
	WScript.Echo
	
    objShell.Exec(strCmd)
	
	' Optional, if there will be many site collections, may want
	' to insert a delay to avoid overloading server memory
	GoSleep(3)

Next

' This function can be used to insert a delay in the processing
' to avoid overloading server memory if there are many
' site collections to be backed up.
Function GoSleep(seconds)
    Dim startTime, endTime, nowTime, dummy
     startTime = DateAdd("s",0,Now)
     endTime = DateAdd("s",seconds,Now)
     nowTime = DateAdd("s",0,Now)
     While endTime > nowTime
         ' Need some commands lin while loop to 
		 ' ensure it actually executes
         nowTime = DateAdd("s",0,Now)
         dummy = Time
     Wend     
End Function
