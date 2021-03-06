using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.DirectoryServices;
using System.Data;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class Service : System.Web.Services.WebService
{
    
    //The LDAP connection string needs to match the domain you'll 
    //be adding users to. For example, the below connection string 
    //applies to a domain called 'test.domain', and will save new 
    //user accounts in the 'NewUsers' organizational unit folder. 
    const string LDAP_CONNECTION_STRING = "LDAP://OU=NewUsers,DC=test,DC=domain"; 
    
    //AD sets account flags by "AND'ing" together various numeric 
    //values stored in HEX. The following are the base-10 
    //integer representations of the HEX values for the flags we 
    //want to set. 
    const int AD_ENABLED = 512; 
    const int AD_DISABLED = 514; 
    const int AD_NEVER_EXPIRE = 65536; 
    
    [WebMethod()] 
    public DataTable AddUserToAD(string strAlias, string strName, string strCompany, string strEmail, string strPhone, string strNotes) 
    { 
        
        string strMsg = ""; 
        
        //Step 1: Verify that alias was provided 
        if (strAlias == "") { 
            
            strMsg = strMsg + "Valid user alias required"; 
        } 
        
        else { 
            
            //Step 2: Instantiate a Directory Entry Object to represent the "Extranet" folder 
            DirectoryEntry adUserFolder = new DirectoryEntry(LDAP_CONNECTION_STRING); 
            DirectoryEntry newADUser = new DirectoryEntry(); 
            DirectoryEntry existingADUser = new DirectoryEntry(); 
            
            //Step 3: Check to make sure the folder is a "organizational unit" object 
            try { 
                if (adUserFolder.SchemaEntry.Name == "organizationalUnit") { 
                    
                    //Create a directory entry to represent the new user 
                    newADUser = adUserFolder.Children.Add("CN=" + strAlias, "User"); 
                    
                    //If already a user with this alias, set the fields to data for 
                    //this user and return message 
                    if (DirectoryEntry.Exists(newADUser.Path)) { 
                        
                        existingADUser = adUserFolder.Children.Find("CN=" + strAlias, "User"); 
                        
                        strName = (string)existingADUser.Properties["displayName"].Value;
                        strCompany = (string)existingADUser.Properties["company"].Value;
                        strNotes = (string)existingADUser.Properties["mail"].Value;
                        strPhone = (string)existingADUser.Properties["telephoneNumber"].Value;
                        strNotes = (string)existingADUser.Properties["comment"].Value; 
                        
                        strMsg = "User '" + strAlias + "' already exists in Active Directory"; 
                    } 
                    
                    else { 
                        
                        //Step 4: Save caller-supplied properties 
                        newADUser.Properties["sAMAccountName"].Add(strAlias + ""); 
                        newADUser.Properties["displayName"].Add(strName + ""); 
                        newADUser.Properties["company"].Add(strCompany + ""); 
                        newADUser.Properties["mail"].Add(strEmail + ""); 
                        newADUser.Properties["telephoneNumber"].Add(strPhone + ""); 
                        newADUser.Properties["comment"].Add(strNotes + ""); 
                        newADUser.Properties["info"].Value = "New SharePoint User"; 
                        newADUser.CommitChanges(); 
                        
                        //Step 5: Set the password using the "Invoke" method. 
                        newADUser.Invoke("setPassword", "P@ssW0rd"); 
                        
                        //Step 6: Enable the user, set account to never expire 
                        newADUser.Properties["userAccountControl"].Value = AD_NEVER_EXPIRE + AD_ENABLED; 
                        newADUser.CommitChanges(); 
                        
                        strMsg = "User '" + strAlias + "' successfully added to Active Directory"; 
                        
                    } 
                    
                } 
            } 
            
            catch (Exception ex) { 
                
                //Step 7: return error message
                strMsg = "User '" + strName + "' could not be added to Active Directory due to the following error: " + ex.Message; 
                
            } 
            
        } 
        
        //Step 8: Construct a dataset to return values 
        DataTable dtReturn = new DataTable("result"); 
        dtReturn.Columns.Add("Alias"); 
        dtReturn.Columns.Add("Name"); 
        dtReturn.Columns.Add("Company"); 
        dtReturn.Columns.Add("Phone"); 
        dtReturn.Columns.Add("Email"); 
        dtReturn.Columns.Add("Notes"); 
        dtReturn.Columns.Add("Message"); 
        
        //Add a single row to the data table to contain 
        //information describing the results of the method call 
        DataRow drReturn = dtReturn.NewRow(); 
        drReturn["Alias"] = strAlias; 
        drReturn["Name"] = strName; 
        drReturn["Company"] = strCompany; 
        drReturn["Phone"] = strPhone; 
        drReturn["Email"] = strEmail; 
        drReturn["Notes"] = strNotes; 
        drReturn["Message"] = strMsg; 
        dtReturn.Rows.Add(drReturn); 
        dtReturn.AcceptChanges(); 
        
        return dtReturn.Copy(); 
        
    } 
}
