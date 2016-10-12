Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Data
Imports System.Xml

Public Class SQLWebPart
    Inherits WebPart

    'Define local variables to contain property values
    Private _connectionString As String = ""
    Private _connectionKey As String = ""
    Private _query As String = ""
    Private _formatUsing As enumFormatUsing = enumFormatUsing.DataGrid
    Private _xsltPath As String = ""
    Private _includeDebugInfo As Boolean = False

    'ENUM types will result in drop-down lists in 
    'the webpart property sheet
    Public Enum enumFormatUsing
        DataGrid = 1
        XSLT = 2
    End Enum

    'Create property to hold SQL connection string
    <Personalizable( _
        PersonalizationScope.Shared), _
        WebBrowsable(), _
        WebDisplayName("Connection String:"), _
        WebDescription("Connection string to use" & _
           " when connecting to SQL source.")> _
    Property ConnectionString() As String
        Get
            Return _connectionString
        End Get

        Set(ByVal Value As String)
            _connectionString = Value
        End Set
    End Property

    'Create property to hold SQL query
    <Personalizable( _
        PersonalizationScope.Shared), _
        WebBrowsable(), _
        WebDisplayName("SQL Query:"), _
        WebDescription("A valid SQL query to execute.")> _
    Property Query() As String
        Get
            Return _query
        End Get

        Set(ByVal Value As String)
            _query = Value
        End Set
    End Property

    'Create property to determine whether datagrid or 
    'XSLT should be used to format output
    <Personalizable( _
        PersonalizationScope.Shared), _
        WebBrowsable(), WebDisplayName("Format Using:"), _
        WebDescription("What method do you want " & _
            "to use to format the results.")> _
    Property FormatUsing() As enumFormatUsing
        Get
            Return _formatUsing
        End Get

        Set(ByVal Value As enumFormatUsing)
            _formatUsing = Value
        End Set
    End Property

    'If XSLT will be used, this property specifies
    'its path
    <Personalizable( _
        PersonalizationScope.Shared), _
        WebBrowsable(), _
        WebDisplayName("XSLT Path:"), _
        WebDescription("If formatting with XSLT, " & _
            "provide full path to XSLT document.")> _
    Property XSLTPath() As String
        Get
            Return _xsltPath
        End Get

        Set(ByVal Value As String)
            _xsltPath = Value
        End Set
    End Property

    'Even though our web parts never have bugs…
    <Personalizable( _
        PersonalizationScope.Shared), _
        WebBrowsable(), _
        WebDisplayName("Include Debug Info?:"), _
        WebDescription("If selected, will " & _
            "display values of web part properties.")> _
    Property IncludeDebugInfo() As Boolean
        Get
            Return _includeDebugInfo
        End Get

        Set(ByVal Value As Boolean)
            _includeDebugInfo = Value
        End Set
    End Property

    'This is where the real work happens!
    Protected Overrides Sub RenderContents( _
          ByVal writer As System.Web.UI.HtmlTextWriter)

        'Process any output from the base class first
        MyBase.RenderContents(writer)

        ' Step 1: Display debug info if requested
        If IncludeDebugInfo Then
            writer.Write("Connection String: " & ConnectionString)
            writer.WriteBreak()
            writer.Write("SQL Query: " & Query)
            writer.WriteBreak()
            writer.Write("Format Using: " & FormatUsing.ToString)
            writer.WriteBreak()
            writer.Write("XSLT Path: " & XSLTPath)
            writer.Write("<hr>")
        End If

        ' Step 2: Query SQL database and return the result set 
        Dim con As New SqlClient.SqlConnection(ConnectionString)
        Try
            con.Open()
        Catch ex As Exception
            writer.Write("<font color='red'>" & ex.Message & "</font>")
            Exit Sub
        End Try

        Dim da As New SqlClient.SqlDataAdapter(Query, con)
        Dim ds As New DataSet

        ' Step 3: copy result set to dataset
        Try
            da.Fill(ds)
        Catch ex As Exception
            writer.Write("<font color='red'>" & ex.Message & "</font>")
            Exit Sub
        End Try

        ' Step 4: Format the output using an XSLT or datagrid
        If FormatUsing = enumFormatUsing.DataGrid Then
            ' Step 5: format using simple DataGrid
            Dim dg As New DataGrid
            dg.DataSource = ds
            dg.DataBind()
            dg.RenderControl(writer)
        Else
            ' Step 6: format using provided XSLT
            Dim xml As New System.Web.UI.WebControls.Xml
            xml.DocumentContent = ds.GetXml
            xml.TransformSource = XSLTPath
            xml.RenderControl(writer)
        End If

    End Sub

End Class
