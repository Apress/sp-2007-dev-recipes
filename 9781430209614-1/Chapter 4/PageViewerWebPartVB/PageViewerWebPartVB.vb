Imports System
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports System.Xml
Imports System.Data


Public Class PageViewerWebPartVB
    Inherits WebPart

    ' Local variable to hold property values
    Private _url As String = ""
    Private _impersonate As Boolean = True
    Private _domain As String = ""
    Private _user As String = ""
    Private _password As String = ""
    Private _debug As Boolean = False

    ' Property to set Url of page to display
    <Personalizable(), _
     WebBrowsable()> _
    Public Property Url() As String
        Get
            Return _url
        End Get
        Set(ByVal value As String)
            _url = value
        End Set
    End Property

    ' If explicit credentials have been requested
    ' the following three properties will
    ' will be used to construct the credentials
    ' to pass to the page
    <Personalizable(), _
     WebBrowsable()> _
    Public Property Domain() As String
        Get
            Return _domain
        End Get
        Set(ByVal value As String)
            _domain = value
        End Set
    End Property

    <Personalizable(), _
     WebBrowsable()> _
    Public Property User() As String
        Get
            Return _user
        End Get
        Set(ByVal value As String)
            _user = value
        End Set
    End Property

    <Personalizable(), _
     WebBrowsable()> _
    Public Property Password() As String
        Get
            Return _password
        End Get
        Set(ByVal value As String)
            _password = value
        End Set
    End Property

    ' Should user be impersonated?
    <Personalizable(), _
     WebBrowsable()> _
    Public Property Impersonate() As Boolean
        Get
            Return _impersonate
        End Get
        Set(ByVal value As Boolean)
            _impersonate = value
        End Set
    End Property

    ' Display debug info?
    <Personalizable(), _
     WebBrowsable()> _
    Public Property Debug() As Boolean
        Get
            Return _debug
        End Get
        Set(ByVal value As Boolean)
            _debug = value
        End Set
    End Property

    ' This is where the HTML gets rendered to the
    ' web part page.
    Protected Overrides Sub RenderContents(ByVal writer As HtmlTextWriter)
        MyBase.RenderContents(writer)

        ' Step 1: If debug info requested, display it
        If Debug Then
            writer.WriteLine("Url: " + Url + "<br/>")
            writer.WriteLine("Impersonate: " + Impersonate.ToString + "<br/>")
            writer.WriteLine("Domain: " + Domain + "<br/>")
            writer.WriteLine("User: " + User + "<br/>")
            writer.WriteLine("Password: " + Password + "<br/>")
            writer.WriteLine("<hr/>")
        End If

        ' Step 2: Make sure Url is provided
        If (Url = "") Then
            writer.WriteLine("<font color='red'>Please enter a valid Url</font>")
            Return
        End If

        ' Step 3: Create a web request to read desired page
        Dim wReq As System.Net.WebRequest
        wReq = System.Net.WebRequest.Create(Url)

        ' Step 4: Set the security as appropriate
        If Impersonate Then
            wReq.Credentials = System.Net.CredentialCache.DefaultCredentials
        Else
            wReq.Credentials = New System.Net.NetworkCredential(User, Password, Domain)
        End If

        ' Step 5: Get the page contents as a string variable
        Try

            Dim wResp As System.Net.WebResponse = wReq.GetResponse
            Dim respStream As System.IO.Stream = wResp.GetResponseStream
            Dim respStreamReader As System.IO.StreamReader = New System.IO.StreamReader(respStream, System.Text.Encoding.ASCII)
            Dim strHTML As String = respStreamReader.ReadToEnd

            ' Step 6: Render the HTML to the web part page
            writer.Write(strHTML)

        Catch e As Exception
            writer.Write(("<font color='red'>" + e.Message))
        End Try

    End Sub

End Class
