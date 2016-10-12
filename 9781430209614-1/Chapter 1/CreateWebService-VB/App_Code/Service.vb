Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls

<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class Service
    Inherits System.Web.Services.WebService

    Private _retVal As String = ""
    Private _siteUrl As String = ""
    Private _webName As String = ""
    Private _title As String = ""
    Private _description As String = ""
    Private _webTemplate As String = ""

    <WebMethod()> _
    Public Function CreateWeb( _
        ByVal strSiteUrl As String, _
        ByVal strWebName As String, _
        ByVal strTitle As String, _
        ByVal strDescription As String, _
        ByVal strWebTemplate As String _
        ) As String

        _siteUrl = strSiteUrl
        _webName = strWebName
        _title = strTitle
        _description = strDescription
        _webTemplate = strWebTemplate

        'Step 1: Verify that all arguments have been passed
        If _siteUrl > "" _
            And _webName > "" _
            And _title > "" _
            And _description > "" _
            And _webTemplate > "" Then

            'Run with permissions of collection administrator
            SPSecurity.RunWithElevatedPrivileges(AddressOf myCreateWeb)
            Return _retVal

        Else

            '2. If some arguments are missing, return error message
            Return "Missing arguments"

        End If

    End Function

    Public Sub myCreateWeb()

        'Step 4: Trap error, if any
        Try
            Dim site As New SPSite(_siteUrl)

            'Step 3: Add the new site to collection 
            Dim web As SPWeb = site.AllWebs.Add(_webName, _title, _description, CType(1033, UInteger), _webTemplate, False, False)

            'Step 5: Return success message
            _retVal = "Successfuly added new site"

            web.Dispose()
            site.Dispose()

        Catch ex As Exception

            'Step 6: Return error message
            _retVal = ex.Message

        End Try

    End Sub

End Class
