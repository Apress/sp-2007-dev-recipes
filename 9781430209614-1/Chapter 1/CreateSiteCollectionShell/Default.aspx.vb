Imports Microsoft.VisualBasic

Partial Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim strExePath As String = """C:\SharePoint Recipe Solutions\Chapter 1\CreateSiteCollection\bin\Debug\CreateSiteCollection.exe"""
        strExePath += " http://localhost/sites/s5"
        strExePath += " ""Test # s5"""
        strExePath += " ""Now is the time..."""
        strExePath += " STS#0"
        strExePath += " Administrator"
        strExePath += " ""Mark Gerow"""
        strExePath += " mgerow@fenwick.com"

        Microsoft.VisualBasic.Shell( _
            strExePath, _
            AppWinStyle.NormalNoFocus, _
            False, _
            -1)
    End Sub

End Class
