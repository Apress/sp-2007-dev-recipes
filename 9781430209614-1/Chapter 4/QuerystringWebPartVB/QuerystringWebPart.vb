Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls
Imports System.Data

Public Class QueryStringWebPartVB
    Inherits WebPart

    ' Define local variables 
    Private _debug As Boolean = False
    Private _formatUsing As enumFormatUsing = enumFormatUsing.DataGrid
    Private _xsltPath As String = ""

    'ENUM types will result in drop-down lists in 
    'the webpart property sheet 
    Public Enum enumFormatUsing
        DataGrid = 1
        XSLT = 2
    End Enum

    ' Display debug info? 
    <Personalizable()> _
    <WebBrowsable()> _
    <WebDisplayName("Debug?")> _
    <WebDescription("Check to cause debug information to be displayed")> _
    Public Property Debug() As Boolean
        Get
            Return _debug
        End Get
        Set(ByVal value As Boolean)
            _debug = value
        End Set
    End Property

    'Create property to determine whether datagrid or 
    'XSLT should be used to format output 
    <Personalizable(PersonalizationScope.[Shared]), _
        WebBrowsable(), WebDisplayName("Format Using:"), _
        WebDescription("What method do you want to use to format the results.")> _
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
    <Personalizable(PersonalizationScope.[Shared]), _
        WebBrowsable(), WebDisplayName("XSLT Path:"), _
        WebDescription("If formatting with XSLT, provide full path to XSLT document.")> _
    Public Property XSLTPath() As String
        Get
            Return _xsltPath
        End Get

        Set(ByVal value As String)
            _xsltPath = value
        End Set
    End Property

    Protected Overloads Overrides Sub RenderContents(ByVal writer As System.Web.UI.HtmlTextWriter)
        MyBase.RenderContents(writer)

        Try
            Dim qs As System.Collections.Specialized.NameValueCollection = Page.Request.QueryString
            If _debug Then

                ' Step 1: Parse the querystring and display 
                If qs.Count > 0 Then
                    writer.Write("<strong>Querystring parameters: </strong>")
                    writer.Write("<blockquote>")
                    For i As Integer = 0 To qs.Count - 1
                        writer.Write(qs.Keys(i) + " = " + qs(i) + "<br/>")
                    Next
                    writer.Write("</blockquote>")
                Else
                    writer.Write("No querystring parameters exist<br/>")
                End If

                ' Step 2: Display web part property values 
                writer.Write("<strong>Format output using:</strong> " + _formatUsing.ToString() + "<br/>")
                writer.Write("<strong>XSLT path:</strong> " + _xsltPath.ToString() + "<br/>")
                writer.Write("<hr/>")

            End If

            ' Step 3: Display items from Client list based on provided ID 
            Dim clientId As String = qs("clientId")
            If clientId IsNot Nothing Then
                displayClientData(clientId, writer)
            Else
                writer.Write("Client ID was not provided in querystring")
            End If

        Catch e As Exception
            writer.Write("<font color='red'>" + e.Message + "</font>")
        End Try

    End Sub

    Private Sub displayClientData(ByVal clientId As String, ByVal writer As System.Web.UI.HtmlTextWriter)
        Try
            ' Step 4: Get handle to current web site and client list 
            Dim web As SPWeb = SPControl.GetContextWeb(Context)
            Dim clients As SPList = web.Lists("Clients")

            ' Step 5: Copy clients' data into a DataTable object 
            ' for easier manipulation 
            Dim dsClients As New DataSet("Clients")
            Dim dtClients As DataTable = clients.Items.GetDataTable()
            dtClients.TableName = "Clients"

            ' Step 6: Filter for the specified client ID 
            Dim dvClients As New DataView()
            dvClients.Table = dtClients
            dvClients.RowFilter = "ClientId = '" + clientId + "'"

            ' Step 7: Determine display mechanism to use 
            If FormatUsing = enumFormatUsing.DataGrid Then

                ' Step 8: Display as DataGrid 
                Dim dgClients As New DataGrid()
                dgClients.DataSource = dvClients
                dgClients.DataBind()
                dgClients.RenderControl(writer)

            Else

                ' Step 9: Format using provided XSLT 
                Dim xml As New System.Web.UI.WebControls.Xml()
                dsClients.Tables.Add(dvClients.ToTable("Clients"))
                xml.DocumentContent = dsClients.GetXml()
                xml.TransformSource = XSLTPath
                xml.RenderControl(writer)

            End If

        Catch ex As Exception
            ' If error occurs, notify end-user 
            writer.WriteLine("<font color='red'><strong>" + ex.Message + "</font>")
        End Try

    End Sub

End Class
