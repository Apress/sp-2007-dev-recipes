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
Imports System.Collections

Public Class _Default
    Inherits System.Web.UI.Page

    ' Hard-coded to save/retrieve properties of root site collection
    Private site As New SPSite("http://localhost")

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim web As SPWeb = site.RootWeb

            ' Step 1: Determine whether a value exists for customer id key
            If web.Properties.ContainsKey("mg_CustomerId") Then

                ' Step 2: Initialize to stored value
                txtCustomerId.Text = web.Properties("mg_CustomerId").ToString

            Else

                ' Step 3: Otherwise set to empty string
                txtCustomerId.Text = ""

            End If

            ' Step 4: Determine whether a value exists for bill flag
            If web.Properties.ContainsKey("mg_BillCustomer") Then

                ' Step 5: Initialize to stored value
                cbBillCustomer.Checked = (web.Properties("mg_BillCustomer").ToString.ToLower = "true")

            Else

                ' Step 6: Otherwise set default value
                cbBillCustomer.Checked = True

            End If

            ' Display list of key/value pairs
            refreshPropertyList()

        End If
    End Sub

    Protected Sub refreshPropertyList()
        Dim web As SPWeb = site.RootWeb
        Dim dtProperties As DataTable = New DataTable("PropertyList")
        Dim dvProperties As DataView = New DataView
        Dim drProperty As DataRow
        dtProperties.Columns.Add("Key")
        dtProperties.Columns.Add("Value")

        ' Step through list of properties, adding
        ' key/value pairs to data table.
        For Each key As Object In web.Properties.Keys
            drProperty = dtProperties.NewRow
            drProperty("Key") = key.ToString
            drProperty("Value") = web.Properties(key.ToString)
            dtProperties.Rows.Add(drProperty)
            dtProperties.AcceptChanges()
        Next

        ' Sort the list and display
        dvProperties.Table = dtProperties
        dvProperties.Sort = "Key"
        gvPropertyList.DataSource = dvProperties
        gvPropertyList.DataBind()

    End Sub

    Protected Sub cmdSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSave.Click
        Dim web As SPWeb = site.RootWeb

        ' Error will occur if AllowUnsafeUpdates property
        ' not set prior to update
        web.AllowUnsafeUpdates = True
        web.Properties("mg_CustomerId") = txtCustomerId.Text
        web.Properties("mg_BillCustomer") = cbBillCustomer.Checked.ToString
        web.Properties.Update()

        ' Redisplay list to reflext changes
        refreshPropertyList()

    End Sub

End Class