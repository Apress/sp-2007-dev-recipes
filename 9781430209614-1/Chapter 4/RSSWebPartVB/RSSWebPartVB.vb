Imports System
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports System.Xml
Imports System.Data

Namespace RSSWebPartVB
    Public Class RSSWebPart
        Inherits WebPart

        ' Local variables to hold web part 
        ' property values 
        Private _url As String
        Private _newPage As Boolean = True
        Private _showDescription As Boolean = True
        Private _showUrl As Boolean = True

        ' Property to determine whether article should 
        ' be opened in same or new page 
        <Personalizable()> _
        <WebBrowsable()> _
        Public Property NewPage() As Boolean
            Get
                Return _newPage
            End Get
            Set(ByVal value As Boolean)
                _newPage = value
            End Set
        End Property

        ' Should Description be displayed? 
        <Personalizable()> _
        <WebBrowsable()> _
        Public Property ShowDescription() As Boolean
            Get
                Return _showDescription
            End Get
            Set(ByVal value As Boolean)
                _showDescription = value
            End Set
        End Property

        ' Should Url be displayed? 
        <Personalizable()> _
        <WebBrowsable()> _
        Public Property ShowUrl() As Boolean
            Get
                Return _showUrl
            End Get
            Set(ByVal value As Boolean)
                _showUrl = value
            End Set
        End Property

        ' Property to set Url of RSS feed 
        <Personalizable()> _
        <WebBrowsable()> _
        Public Property Url() As String
            Get
                Return _url
            End Get
            Set(ByVal value As String)
                _url = value
            End Set
        End Property

        ' This is where the HTML gets rendered to the 
        ' web part page. 
        Protected Overloads Overrides Sub RenderContents(ByVal writer As HtmlTextWriter)
            MyBase.RenderContents(writer)

            ' Step 1: Ensure Url property has been provided 
            If Url <> "" Then

                ' Display heading with RSS location url 
                If ShowUrl Then
                    writer.WriteLine("<hr/>")
                    writer.WriteLine("<span style='font-size: larger;'>")
                    writer.WriteLine("Results for: ")
                    writer.WriteLine("<strong>")
                    writer.WriteLine(Url)
                    writer.WriteLine("</strong>")
                    writer.WriteLine("</span>")
                    writer.WriteLine("<hr/>")
                End If
                displayRSSFeed(writer)

            Else

                ' Tell user they need to fill in the Url property 
                writer.WriteLine("<font color='red'>RSS Url cannot be blank</font>")

            End If

        End Sub

        Private Sub displayRSSFeed(ByVal writer As HtmlTextWriter)
            Try
                ' Step 2: Read the RSS feed into memory 
                Dim wReq As System.Net.WebRequest
                wReq = System.Net.WebRequest.Create(Url)
                wReq.Credentials = System.Net.CredentialCache.DefaultCredentials

                ' Return the response. 
                Dim wResp As System.Net.WebResponse = wReq.GetResponse()
                Dim respStream As System.IO.Stream = wResp.GetResponseStream()

                ' Load RSS stream into a data set for easier processing 
                Dim dsXML As New DataSet()
                dsXML.ReadXml(respStream)

                ' Step 4: Loop through all items returned, displaying results 
                Dim target As String = ""
                If NewPage Then
                    target = "target='_new'"
                End If

                For Each item As DataRow In dsXML.Tables("item").Rows

                    ' Step 5: Write the title, link and description to page 
                    writer.WriteLine("<a href='" + item("link") + "' " + target + ">" + item("title") + "</a>" + "<br/>" + "<span style='color:silver'>" + item("pubDate") + "</span>" + "<br/>")

                    If ShowDescription Then
                        writer.WriteLine("<br/>" + item("description"))
                    End If

                    writer.WriteLine("<hr/>")
                Next

            Catch ex As Exception

                ' Step 3: If error occurs, notify end-user 
                writer.WriteLine("<font color='red'><strong>" + "An error occurred while attempting to process the selected RSS feed. " + "Please verify that the url provided references a valid RSS page.")

            End Try

        End Sub

    End Class

End Namespace