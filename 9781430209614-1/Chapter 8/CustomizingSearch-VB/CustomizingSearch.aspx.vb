Imports System
Imports System.Data
Imports System.Configuration
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Microsoft.Office.Server
Imports Microsoft.Office.Server.Search
Imports Microsoft.Office.Server.Search.Query
Imports Microsoft.Office.Server.Search.Administration

Partial Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub cmdSearch_Click(ByVal sender As Object, ByVal e As EventArgs)
        If txtSearch.Text <> "" Then
            performSearch()
        Else
            lblMsg.Text = "Please enter a search string"
        End If
    End Sub

    Private Sub performSearch()
        ' Step 1: Get a handle to the Shared Services Search context 
        Dim context As ServerContext = ServerContext.GetContext("SharedServices1")

        ' Step 2: Construct a keyword search query 
        Dim kwq As New KeywordQuery(context)
        kwq.ResultTypes = ResultType.RelevantResults
        kwq.EnableStemming = True
        kwq.TrimDuplicates = True
        kwq.QueryText = txtSearch.Text
        kwq.Timeout = 60000
        kwq.RowLimit = 1000
        kwq.KeywordInclusion = KeywordInclusion.AllKeywords

        ' Step 3: Get the results to a DataTable 
        Dim results As ResultTableCollection = kwq.Execute()
        Dim resultTable As ResultTable = results(ResultType.RelevantResults)
        Dim dtResults As New DataTable()
        dtResults.Load(resultTable)

        ' Step 4: Format summary 
        For Each drResult As DataRow In dtResults.Rows
            drResult("HitHighlightedSummary") = formatSummary(drResult("HitHighlightedSummary").ToString())
        Next

        ' Step 6: Write out table of results 
        gridSearch.DataSource = dtResults
        gridSearch.DataBind()

        ' Step 7: Inform the user how many hits were found 
        lblMsg.Text = dtResults.Rows.Count.ToString() + " hits"
    End Sub

    ' Step 5: Highlight first 4 hits 
    ' SharePoint Search places <c[#]> tags around the 
    ' first 10 words in the summary that match 
    ' a keyword search term. Here I just find 
    ' the first four and replace them with 
    ' a <SPAN> element to show the hits in 
    ' bold with a yellow background 
    Private Function formatSummary(ByVal strSummary As String) As String
        strSummary = strSummary.Replace("<c0>", "<span style='font-weight:bold; background-color:yellow'>")
        strSummary = strSummary.Replace("</c0>", "</span>")

        strSummary = strSummary.Replace("<c1>", "<span style='font-weight:bold; background-color:yellow'>")
        strSummary = strSummary.Replace("</c1>", "</span>")

        strSummary = strSummary.Replace("<c2>", "<span style='font-weight:bold; background-color:yellow'>")
        strSummary = strSummary.Replace("</c2>", "</span>")

        strSummary = strSummary.Replace("<c3>", "<span style='font-weight:bold; background-color:yellow'>")
        strSummary = strSummary.Replace("</c3>", "</span>")

        Return strSummary
    End Function

End Class