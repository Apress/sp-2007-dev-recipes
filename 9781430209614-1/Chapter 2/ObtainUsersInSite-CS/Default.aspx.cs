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
using System.Collections;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string siteUrl;
        string webName = "";
        int fontSize = 14;

        // Validate site url
        try
        {
            siteUrl = Request.QueryString["siteUrl"].ToString();
        }
        catch
        {
            Response.Write("<div style='font-size: " + fontSize + "pt; font-weight: bold; font-color: red'>");
            Response.Write("Please provide a value for 'siteUrl' in the querystring");
            Response.Write("</div>");
            return;
        }

        // Validate web name
        try
        {
            webName = Request.QueryString["webName"].ToString();
        }
        catch
        {
            Response.Write("<div style='font-size: " + fontSize + "pt; font-weight: bold; font-color: red'>");
            Response.Write("Please provide a value for 'webName' in the querystring");
            Response.Write("</div>");
            return;
        }
        //}

        // Display banner and return link
        Response.Write("<H1>List of users for web site \'" + siteUrl + "/" + webName + "</H1>");

        SPSite site = new SPSite(siteUrl);
        ArrayList ADGroups = new ArrayList();
        DataTable userList = new DataTable();
        userList = GetAllSiteUsers(siteUrl, webName);

        string heading = "The following report displays all members of the '" + site.AllWebs[webName].Title + "' Site as of "
            + DateTime.Now.ToLocalTime()
            + "<br/><br/>";

        Response.Write("<div style='font-size: " + fontSize + "; font-weight: bold'>");
        Response.Write(heading);
        Response.Write("<hr/>");
        Response.Write("</div>");

        // Display users in all groups, and who are not members of any group.
        string prevGroup = "...";
        Table table = new Table();

        foreach (DataRow userRow in userList.Rows)
        {

            // If a new GROUP, display heading info
            if (prevGroup != (string)userRow["GroupName"])
            {
                Response.Write("<br/>");
                if ((string)userRow["GroupName"].ToString() != "")
                {
                    Response.Write("<strong>Group: " + userRow["GroupName"] + "  [" + userRow["GroupRoles"] + "]</strong><br/><br/>");
                }
                else
                {
                    Response.Write("<strong>The following users have been given explicit permissions in this Site</strong><br/><br/>");
                }
                prevGroup = (string)userRow["GroupName"];
            }

            if (userRow["UserName"].ToString() != "") 
            {
                Response.Write(userRow["UserName"]);
            }
            else
            {
                Response.Write(" (" + userRow["UserAlias"] + ") ");
            }

            if ((string)userRow["UserRoles"] != "")
            {
                Response.Write("  [" + userRow["UserRoles"] + "] ");
            }

            if ((string)userRow["IsADGroup"] != "False")
            {
                Response.Write("<font color='red'> ** Active Directory Security Group</font>");
            }
            Response.Write("<br/>");
        }
        Response.Write("</div>");
    }

    private DataTable GetAllSiteUsers(string siteUrl, string webName)
    {
        // Step1: Open the web site to process
        SPSite site = new SPSite(siteUrl);
        SPWeb web = site.AllWebs[webName];

        // Step 2: Create a data table to hold list
        //  of site users
        DataTable userList = new DataTable("UserList");
        DataRow userRow;
        userList.Columns.Add("GroupName");
        userList.Columns.Add("GroupRoles");
        userList.Columns.Add("UserAlias");
        userList.Columns.Add("UserName");
        userList.Columns.Add("UserRoles");
        userList.Columns.Add("UserCompany");
        userList.Columns.Add("IsADGroup");

        // Step 3: Iterate through site groups
        foreach (SPGroup group in web.SiteGroups)
        {
            // Step 4:Get list of all users in this group
            //  and add to data table
            foreach (SPUser user in group.Users)
            {
                userRow = userList.NewRow();
           
                userRow["GroupName"] = group.Name;
                userRow["GroupRoles"] = GetRoles(group);
                userRow["UserName"] = user.Name;
                userRow["UserAlias"] = user.LoginName.ToString();
                userRow["UserRoles"] = GetRoles(user);
                userRow["IsADGroup"] = user.IsDomainGroup.ToString();

                userList.Rows.Add(userRow);
            }
        }

        // Step 5: Get users who have been assigned 
        //  explicit permissions
        foreach (SPUser user in web.Users)
        {
            if (user.Groups.Count == 0 || GetRoles(user) != "")
            {
                userRow = userList.NewRow();

                userRow["GroupName"] = "";
                userRow["GroupRoles"] = "";
                userRow["UserName"] = user.Name;
                userRow["UserAlias"] = user.LoginName;
                userRow["UserRoles"] = GetRoles(user);
                userRow["IsADGroup"] = user.IsDomainGroup.ToString();

                userList.Rows.Add(userRow);
            }
        }

        return userList;
    }

    // Note: the SPUser.Roles collection has been
    //  deprecated in WSS 3.0, but it's still the
    //  simplest way to access roles assigned to a
    //  user.
    string GetRoles(SPPrincipal gu)
    {
        string roleInfo = "";
        foreach (SPRole role in gu.Roles)
        {
            if (roleInfo != "")
            {
                roleInfo = roleInfo + ",";
            }
            roleInfo = roleInfo + role.Name.ToString();
        }
        return roleInfo;
    }
}
