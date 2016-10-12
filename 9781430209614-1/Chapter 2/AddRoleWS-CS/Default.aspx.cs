using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Microsoft.SharePoint;
using System.Xml;

public partial class _Default : System.Web.UI.Page 
{

    private UserGroupService.UserGroup objUserGroup = new UserGroupService.UserGroup();

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    private ulong GetPermissionFlags(string strRoleName)
    {
        // Step 1: Default to NO permissions
        ulong permissionFlags = (ulong)SPBasePermissions.EmptyMask;

        // Step 2: Get list of all current roles for this web site
        XmlNode xnRoles = objUserGroup.GetRoleCollectionFromWeb();

        // Step 3: Even though we're using the web service to update
        //  the roles collection, we can use the built-in enum
        //  type to get the numeric values of the various base
        //  permissions.
        SPBasePermissions enumBasePermissions = new SPBasePermissions();
        string[] arrBasePermissionNames = System.Enum.GetNames(enumBasePermissions.GetType());
        ulong[] arrBasePermissionValues = (ulong[])System.Enum.GetValues(enumBasePermissions.GetType());

        // Step 4: loop through all current roles in target site
        //  finding the role for which we want to duplicate permission
        //  flags.
        foreach (XmlNode xnRole in xnRoles.FirstChild.ChildNodes)
        {
            if (xnRole.Attributes["Name"].Value.ToString().ToLower() == strRoleName.ToLower())
            {
                // Turn the comma-delimited list of base permissing names into
                // an array so we can iterate through them
                string[] arrPermission = xnRole.Attributes["BasePermissions"].Value.ToString().Split(',');

                // Iterate through the complete list of base permissions to find the entry
                // that matches the base permission from our template role
                for (int i = 0; i < arrPermission.Length; i++)
                    for (int j = 0; j < arrBasePermissionNames.Length; j++)

                        // When we've found our base permission, "OR" its
                        // numeric value with that of any other base permissions
                        // to create the complete set of values
                        if (arrPermission[i].Trim() == arrBasePermissionNames[j])
                            permissionFlags = permissionFlags | arrBasePermissionValues[j];
            }
        }
        return permissionFlags;
    }
    protected void cmdAddRole_Click(object sender, EventArgs e)
    {
        try
        {
            // Point the UserGroup web service to our target site collection 
            // and web site
            objUserGroup.Url = txtSiteUrl.Text + "/" + txtWebName.Text + "/_vti_bin/usergroup.asmx";
            objUserGroup.Credentials = System.Net.CredentialCache.DefaultCredentials;

            // Get the permission flags of the role to be cloned
            ulong permissionFlags = GetPermissionFlags(rblTemplateRole.SelectedValue);

            // Create the new role
            objUserGroup.AddRoleDef(txtRoleName.Text, txtRoleDefinition.Text, permissionFlags);

            // Display success message
            lblReturnMsg.Text = "Successfully added '" + txtRoleName.Text + "' role.";
        }

        catch (Exception ex)
        {
            lblReturnMsg.Text = "Error: " + ex.Message;
        }
    }
}
