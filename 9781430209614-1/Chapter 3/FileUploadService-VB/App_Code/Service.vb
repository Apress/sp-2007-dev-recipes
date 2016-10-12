Imports System
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls

<WebService([Namespace]:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
Public Class Service
    Inherits System.Web.Services.WebService

    'Uncomment the following line if using designed components 
    'InitializeComponent(); 
    Public Sub New()
    End Sub

    <WebMethod()> _
    Public Function UploadFile2SharePoint(ByVal fileName As String, ByVal fileContents As Byte(), ByVal siteUrl As String, ByVal webName As String, ByVal pathFolder As String) As String
        ' Step 1: Make sure a valid file has been passed to the service method 
        If fileContents Is Nothing Then
            Return "Missing File"
        End If
        Try
            ' Step 2: Open the target site and web 
            Dim site As New SPSite(siteUrl)
            Dim web As SPWeb = site.AllWebs(webName)

            ' Step 3: Open the folder to hold the document 
            Dim folder As SPFolder = web.GetFolder(EnsureParentFolder(web, pathFolder + "/" + fileName))
            Dim boolOverwrite As Boolean = True

            ' Step 4: Add the file 
            Dim file As SPFile = folder.Files.Add(fileName, fileContents, boolOverwrite)

            ' Step 5: Declare victory! 

            Return "'" + file.Name + "' successfuly written to '" + file.Item.Url + "'"
        Catch ex As System.Exception
            Return ex.Message
        End Try
    End Function

    ' This is a stock function from the WSS SDK to make 
    ' sure that a folder path exists before we try to upload the 
    ' file. 
    Public Function EnsureParentFolder(ByVal parentSite As SPWeb, ByVal destinUrl As String) As String
        destinUrl = parentSite.GetFile(destinUrl).Url

        Dim index As Integer = destinUrl.LastIndexOf("/")
        Dim parentFolderUrl As String = String.Empty

        If index > -1 Then
            parentFolderUrl = destinUrl.Substring(0, index)

            Dim parentFolder As SPFolder = parentSite.GetFolder(parentFolderUrl)

            If Not parentFolder.Exists Then
                Dim currentFolder As SPFolder = parentSite.RootFolder

                For Each folder As String In parentFolderUrl.Split("/"c)
                    currentFolder = currentFolder.SubFolders.Add(folder)
                Next
            End If
        End If
        Return parentFolderUrl
    End Function

End Class