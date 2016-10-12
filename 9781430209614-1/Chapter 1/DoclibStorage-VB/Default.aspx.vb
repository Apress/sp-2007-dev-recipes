Imports System
Imports System.Data
Imports System.Configuration
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls
Imports System.Text

Partial Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        'Step 1: Open site collection and web site 
        ' for which we want to report on list storage 
        Dim site As New SPSite("http://localhost")
        Dim web As SPWeb = site.RootWeb

        'Step 2: Get collection of all lists 
        Dim lists As SPListCollection = web.Lists

        'Step 3: iterate through all lists, finding 
        ' those which are document librarys 
        Dim dtListItems As DataTable
        For Each list As SPList In lists
            'Step 4: Is this a document library 
            If list.BaseTemplate = SPListTemplateType.DocumentLibrary Then

                'Step 5: Get list of all documents in library 
                dtListItems = list.Items.GetDataTable()

                'Step 6: is there at least one document in 
                ' the library? 
                If dtListItems IsNot Nothing Then

                    'Step 7: Add heading 
                    Dim lbl As New Label()
                    lbl.Text = "<h1>" + list.Title + "</h1>"
                    Me.Controls.Add(lbl)

                    'Step 8: Select just the desired columns 
                    dtListItems = FormatTable(dtListItems)

                    'Step 9: Create XML representation of document list 
                    Dim sb As New StringBuilder()
                    Dim sw As New System.IO.StringWriter(sb)
                    dtListItems.WriteXml(sw)

                    'Step 10: Format XML using XSLT 
                    Dim xmlListItems As New Xml()
                    xmlListItems.DocumentContent = sb.ToString()
                    xmlListItems.TransformSource = "Recipes.xsl"

                    Me.Controls.Add(xmlListItems)

                End If
            End If
        Next
    End Sub

    Private Function FormatTable(ByVal dtListItems As DataTable) As DataTable
        Dim dtMyList As New DataTable("ListItems")
        dtMyList.Columns.Add("ID")
        dtMyList.Columns.Add("FileName")
        dtMyList.Columns.Add("FileSize")
        For Each drListItem As DataRow In dtListItems.Rows
            Dim drMyListItem As DataRow = dtMyList.NewRow()
            drMyListItem("ID") = drListItem("ID")
            drMyListItem("FileName") = drListItem("LinkFileName")
            drMyListItem("FileSize") = drListItem("FileSizeDisplay")
            dtMyList.Rows.Add(drMyListItem)
            dtMyList.AcceptChanges()
        Next
        Return dtMyList
    End Function

End Class