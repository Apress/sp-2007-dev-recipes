Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts

Namespace ConnectablePageViewerVB

    ' Step 1: Create the Interface 
    ' ---------------------------- 
    ' The interface is the glue between the "provider" web part, 
    ' that sends data to the "consumer" web page. It provides 
    ' a structure in which to pass the data. In this case 
    ' we're just passing a single string value that represents 
    ' the Url to display, but we could pass multiple data 
    ' items by providing multiple properties 
    Public Interface IUrl
        ReadOnly Property Url() As String
    End Interface

    ' Step 2: Create the provider web part class 
    ' ------------------------------------------ 
    ' The "provider" web part will display a drop-down list 
    ' or site-name/url pairs. When the user selects a value 
    ' the selected Url will be passed to the "consumer". 
    Public Class UrlProvider
        Inherits WebPart
        Implements IUrl

        Private ddlUrl As DropDownList = Nothing
        Private _urls As String = "Microsoft;http://www.microsoft.com;Yahoo!;http://www.yahoo.com"

        ' The "Urls" property will store a semi-conlon delimeted 
        ' list of site-name/url pairs to populate the drop-down list 
        <Personalizable()> _
        <WebBrowsable()> _
        Public Property Urls() As String
            Get
                Return _urls
            End Get
            Set(ByVal value As String)
                _urls = value
            End Set
        End Property

        ' Step 3: Override the “CreateChildControls()” method 
        ' --------------------------------------------------- 
        ' The CreateChildControls() base method is called 
        ' to populate the drop-down list of sites and 
        ' add to the web part output 
        Protected Overloads Overrides Sub CreateChildControls()
            MyBase.CreateChildControls()

            Try
                ' Create the drop-down list of urls from 
                ' the parsed string in "Urls" property 
                Dim arrUrls As String() = _urls.Split(";"c)
                Dim li As ListItem
                ddlUrl = New DropDownList()
                ddlUrl.Items.Add(New ListItem("[Please select a Url]", ""))
                Dim i As Integer = 0
                While i < arrUrls.Length
                    li = New ListItem(arrUrls(i), arrUrls(i + 1))
                    ddlUrl.Items.Add(li)
                    i = i + 2
                End While
                ddlUrl.Items(0).Selected = True
                ddlUrl.AutoPostBack = True

                Me.Controls.Add(ddlUrl)

            Catch ex As Exception
                Dim lbl As New Label()
                lbl.Text = ex.Message
                Me.Controls.Add(lbl)
            End Try

        End Sub

        ' Step 4: Define any methods required by the interface 
        ' ---------------------------------------------------- 
        ' This is the single method that was 
        ' specified in the Interface, and must be provided 
        ' to pass the selected url to the "consumer" web 
        ' part 
        Public ReadOnly Property Url() As String Implements IUrl.Url
            Get
                Return ddlUrl.SelectedValue.ToString()
            End Get
        End Property

        ' Step 5: Define and “decorate” the ConnectionProvider method 
        ' ----------------------------------------------------------- 
        ' This method is required to wire-up the 
        ' "provider" with one or more "consumers". 
        ' Note the "ConnectionProvider" decoration, 
        ' that tells .NET to make this the provider's 
        ' connection point 
        <ConnectionProvider("Url Provider")> _
        Public Function GetUrl() As IUrl
            Return Me
        End Function

    End Class

    ' Step 6: Define the consumer web part class 
    ' ------------------------------------------ 
    ' This class defines the "consumer" web part that will 
    ' obtain the url from the "provider" 
    Public Class ConnectablePageViewer
        Inherits WebPart
        Private _url As String = ""

        ' Step 7: Override either or both the CreateChildControls() and/or 
        ' RenderContents() base methods 
        ' ---------------------------------------------------------------- 
        ' In the RenderContents() method we get the url value 
        ' which has been written to the _url local variable by 
        ' the "UrlConsumer()" method that automatically fires 
        ' when this web part is wired-up with a "provider" 
        Protected Overloads Overrides Sub RenderContents(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.RenderContents(writer)
            Try
                If _url <> "" Then
                    ' Create an <IFRAME> HTML tag and set the 
                    ' source to the selected url 
                    writer.Write("Opening page: " + _url)
                    writer.Write("<hr/>")
                    writer.Write("<div>")
                    writer.Write("<iframe src='" + _url + "' width='100%' height='100%'></iframe>")
                    writer.Write("</div>")
                Else
                    writer.Write("Please select a Url from the provider.")
                End If
            Catch ex As Exception
                writer.Write(ex.Message)
            End Try
        End Sub

        ' Step 8: Define a ConnectionConsumer() method to receive 
        ' data from the provider 
        ' ------------------------------------------------------- 
        ' The UrlConsumer() method is wired-up using the 
        ' "ConnectionConsumer()" decoration, that tells 
        ' .NET to automatically fire this method when 
        ' the consumer is connected to a provider 
        <ConnectionConsumer("Url Consumer")> _
        Public Sub UrlConsumer(ByVal url As IUrl)
            Try
                _url = url.Url
                ' No op 
            Catch ex As Exception
            End Try
        End Sub

    End Class

End Namespace