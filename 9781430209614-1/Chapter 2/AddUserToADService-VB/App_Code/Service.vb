Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.DirectoryServices
Imports System.Data

<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class AddUserToADService
    Inherits System.Web.Services.WebService

    'The LDAP connection string needs to match the domain you'll
    'be adding users to.  For example, the below connection string
    'applies to a domain called 'test.domain', and will save new
    'user accounts in the 'NewUsers' organizational unit folder.
    Const LDAP_CONNECTION_STRING = "LDAP://OU=NewUsers,DC=test,DC=domain"

    'AD sets account flags by "AND'ing" together various numeric
    'values stored in HEX.  The following are the base-10
    'integer representations of the HEX values for the flags we
    'want to set.
    Const AD_ENABLED = 512
    Const AD_DISABLED = 514
    Const AD_NEVER_EXPIRE = 65536

    <WebMethod()> _
    Public Function AddUserToAD(ByVal strAlias As String, _
            ByVal strName As String, _
            ByVal strCompany As String, _
            ByVal strEmail As String, _
            ByVal strPhone As String, _
            ByVal strNotes As String) As DataTable

        Dim strMsg As String = ""

        'Step 1: Verify that alias was provided
        If strAlias = "" Then

            strMsg = strMsg & "Valid user alias required"

        Else

            'Step 2: Instantiate a Directory Entry Object to represent the "Extranet" folder
            Dim adUserFolder As New DirectoryEntry(LDAP_CONNECTION_STRING)
            Dim newADUser As New DirectoryEntry
            Dim existingADUser As New DirectoryEntry

            'Step 3: Check to make sure the folder is a "organizational unit" object
            Try
                If adUserFolder.SchemaEntry.Name = "organizationalUnit" Then

                    'Create a directory entry to represent the new user
                    newADUser = adUserFolder.Children.Add("CN=" & strAlias, "User")

                    'If already a user with this alias, set the fields to data for 
                    'this user and return message
                    If DirectoryEntry.Exists(newADUser.Path) Then

                        existingADUser = adUserFolder.Children.Find("CN=" & strAlias, "User")

                        strName = existingADUser.Properties("displayName").Value
                        strCompany = existingADUser.Properties("company").Value
                        strNotes = existingADUser.Properties("mail").Value
                        strPhone = existingADUser.Properties("telephoneNumber").Value
                        strNotes = existingADUser.Properties("comment").Value

                        strMsg = "User '" & strAlias & "' already exists in Active Directory"

                    Else

                        'Step 4: Save caller-supplied properties
                        newADUser.Properties("sAMAccountName").Add(strAlias & "")
                        newADUser.Properties("displayName").Add(strName & "")
                        newADUser.Properties("company").Add(strCompany & "")
                        newADUser.Properties("mail").Add(strEmail & "")
                        newADUser.Properties("telephoneNumber").Add(strPhone & "")
                        newADUser.Properties("comment").Add(strNotes & "")
                        newADUser.Properties("info").Value = "New SharePoint User"
                        newADUser.CommitChanges()

                        'Step 5: Set the password using the "Invoke" method.
                        newADUser.Invoke("setPassword", "P@ssW0rd")

                        'Step 6: Enable the user, set account to never expire
                        newADUser.Properties("userAccountControl").Value = AD_NEVER_EXPIRE + AD_ENABLED
                        newADUser.CommitChanges()

                        strMsg = "User '" & strAlias & "' successfully added to Active Directory"

                    End If

                End If

            Catch ex As Exception

                'Step 7: return error message
                strMsg = "User '" & strName & "' could not be added to Active Directory due to the following error: " & ex.Message

            End Try

        End If

        'Step 8: Construct a dataset to return values
        Dim dtReturn As New Data.DataTable("result")
        dtReturn.Columns.Add("Alias")
        dtReturn.Columns.Add("Name")
        dtReturn.Columns.Add("Company")
        dtReturn.Columns.Add("Phone")
        dtReturn.Columns.Add("Email")
        dtReturn.Columns.Add("Notes")
        dtReturn.Columns.Add("Message")

        'Add a single row to the data table to contain
        'information describing the results of the method call
        Dim drReturn As Data.DataRow = dtReturn.NewRow
        drReturn("Alias") = strAlias
        drReturn("Name") = strName
        drReturn("Company") = strCompany
        drReturn("Phone") = strPhone
        drReturn("Email") = strEmail
        drReturn("Notes") = strNotes
        drReturn("Message") = strMsg
        dtReturn.Rows.Add(drReturn)
        dtReturn.AcceptChanges()

        Return dtReturn.Copy

    End Function

End Class
