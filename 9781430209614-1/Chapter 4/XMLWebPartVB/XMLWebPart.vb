Imports System
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports System.Xml
Imports System.Data

Public Class XMLWebPart
    Inherits WebPart
    ' Local variable to hold property values 
    Private _url As String = ""
    Private _impersonate As Boolean = True
    Private _domain As String = ""
    Private _user As String = ""
    Private _password As String = ""
    Private _debug As Boolean = False
    Private _formatUsing As enumFormatUsing = enumFormatUsing.DataGrid
    Private _xsltPath As String = ""

    'ENUM types will result in drop-down lists in 
    'the webpart property sheet 
    Public Enum enumFormatUsing
        DataGrid = 1
        XSLT = 2
    End Enum

    ' Property to set Url of source XML document 
    <Personalizable()> _
    <WebBrowsable()> _
    <WebDisplayName("Url of XML document")> _
    Public Property Url() As String
        Get
            Return _url
        End Get
        Set(ByVal value As String)
            _url = value
        End Set
    End Property

    'Create property to determine whether datagrid or 
    'XSLT should be used to format output 
    <Personalizable(PersonalizationScope.[Shared]), WebBrowsable(), WebDisplayName("Format Using:"), WebDescription("What method do you want " + "to use to format the results.")> _
    Public Property FormatUsing() As enumFormatUsing
        Get
            Return _formatUsing
        End Get

        Set(ByVal value As enumFormatUsing)
            _formatUsing = value
        End Set
    End Property

    'If XSLT will be used, this property specifies 
    'its server-relative path 
    <Personalizable(PersonalizationScope.[Shared]), WebBrowsable(), WebDisplayName("XSLT Path:"), WebDescription("If formatting with XSLT, " + "provide full path to XSLT document.")> _
    Public Property XSLTPath() As String
        Get
            Return _xsltPath
        End Get

        Set(ByVal value As String)
            _xsltPath = value
        End Set
    End Property

    ' If explicit credentials have been requested 
    ' the following three properties, Domain, User, and 
    ' Password will be used to construct the credentials 
    ' to pass to the page 
    <Personalizable()> _
    <WebBrowsable()> _
    Public Property Domain() As String
        Get
            Return _domain
        End Get
        Set(ByVal value As String)
            _domain = value
        End Set
    End Property

    <Personalizable()> _
    <WebBrowsable()> _
    Public Property User() As String
        Get
            Return _user
        End Get
        Set(ByVal value As String)
            _user = value
        End Set
    End Property

    <Personalizable()> _
    <WebBrowsable()> _
    Public Property Password() As String
        Get
            Return _password
        End Get
        Set(ByVal value As String)
            _password = value
        End Set
    End Property

    ' If this option is checked, the web part will use 
    ' the default credentials of the user viewing 
    ' the web part page. 
    <Personalizable()> _
    <WebBrowsable()> _
    Public Property Impersonate() As Boolean
        Get
            Return _impersonate
        End Get
        Set(ByVal value As Boolean)
            _impersonate = value
        End Set
    End Property

    ' Display debug info? 
    <Personalizable()> _
    <WebBrowsable()> _
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
    Protected Overloads Overrides Sub RenderContents(ByVal writer As HtmlTextWriter)
        MyBase.RenderContents(writer)

        ' Step 1: Ensure Url property has been provided 
        If Url <> "" Then
            ' Step 2: If debug info requested, display it 
            If Debug Then
                writer.WriteLine("Url: " + Url + "<br/>")
                writer.WriteLine("Impersonate: " + Impersonate.ToString() + "<br/>")
                writer.WriteLine("Domain: " + Domain + "<br/>")
                writer.WriteLine("User: " + User + "<br/>")
                writer.WriteLine("Password: " + Password + "<br/>")
                writer.WriteLine("Format using: " + FormatUsing.ToString() + "<br/>")
                writer.WriteLine("<hr/>")
            End If

            ' Call helper function to render data as HTML to page 
            displayXML(writer)
        Else
            ' Step 3: Tell user they need to fill in the Url property 
            writer.WriteLine("<font color='red'>Source XML url cannot be blank</font>")
        End If
    End Sub

    Private Sub displayXML(ByVal writer As HtmlTextWriter)
        Try
            ' Step 4: Read the XML document into memory 
            Dim wReq As System.Net.WebRequest
            wReq = System.Net.WebRequest.Create(Url)

            ' Step 5: Set the security as appropriate 
            If Impersonate Then
                wReq.Credentials = System.Net.CredentialCache.DefaultCredentials
            Else
                wReq.Credentials = New System.Net.NetworkCredential(User, Password, Domain)
            End If

            wReq.Credentials = System.Net.CredentialCache.DefaultCredentials

            ' Step 6: Return the response. 
            Dim wResp As System.Net.WebResponse = wReq.GetResponse()
            Dim respStream As System.IO.Stream = wResp.GetResponseStream()

            ' Step 7: Load XML stream into a data set for easier processing 
            Dim dsXML As New DataSet()
            dsXML.ReadXml(respStream)

            ' Step 8: determine display mechanism to use 
            If FormatUsing = enumFormatUsing.DataGrid Then
                ' Step 9: Loop through each table in the DataSet, 
                ' displaying each in a DataGrid 
                Dim dgXML As DataGrid
                Dim lbl As Label
                For Each dtXML As DataTable In dsXML.Tables
                    ' Display table name 
                    lbl = New Label()
                    lbl.Text = "<br/><strong>" + dtXML.TableName.ToUpper() + "</strong><br/><br/>"
                    lbl.RenderControl(writer)

                    ' Now display the data 
                    dgXML = New DataGrid()
                    dgXML.DataSource = dtXML
                    dgXML.DataBind()
                    dgXML.RenderControl(writer)
                Next
            Else
                ' Step 10: Format using provided XSLT 
                Dim xml As New System.Web.UI.WebControls.Xml()
                xml.DocumentContent = dsXML.GetXml()
                xml.TransformSource = XSLTPath
                xml.RenderControl(writer)
            End If

        Catch ex As Exception
            ' If error occurs, notify end-user 
            writer.WriteLine("<font color='red'><strong>" + ex.Message + "</font>")
        End Try
    End Sub

End Class
