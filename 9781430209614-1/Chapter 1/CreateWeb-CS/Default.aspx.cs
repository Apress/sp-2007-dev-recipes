using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class _Default : System.Web.UI.Page {
    
    protected void Page_Load(object sender, System.EventArgs e) {
        try {
            if (!IsPostBack) {
                // Step 1: Get list of templates for the selected web application
                SitesService.Sites objSites = new SitesService.Sites();
                objSites.Credentials = System.Net.CredentialCache.DefaultCredentials;
                SitesService.Template[] arrTemplates;
                uint templateCount = objSites.GetSiteTemplates(1033, out arrTemplates);
                int i;
                ListItem listItem;
                ddlTemplate.Items.Clear();
                for (i = 0; (i 
                            <= (arrTemplates.Length - 1)); i++) {
                    // Don't include hidden templates, which are not intended for interactive use
                    if (!arrTemplates[i].IsHidden) {
                        listItem = new ListItem(arrTemplates[i].Title, arrTemplates[i].Name);
                        ddlTemplate.Items.Add(listItem);
                    }
                }
                ddlTemplate.Enabled = true;
                lblMessage.Text = "";
            }
        }
        catch (Exception ex) {
            lblMessage.Text = ex.Message;
        }
    }
    
    protected void cmdCreateWeb_Click1(object sender, EventArgs e)
    {
        try
        {

            // Step 2: Make sure all necessary data is provided
            if (txtSiteCollPath.Text != ""
                  && txtWebName.Text != ""
                  && txtTitle.Text != ""
                  && txtDescription.Text != ""
                  && ddlTemplate.SelectedValue != "")
            {

                // Step 4: Add new site collection 
                CreateWebService_CS.Service objCreateWebService = new CreateWebService_CS.Service();
                objCreateWebService.Credentials = System.Net.CredentialCache.DefaultCredentials;
                objCreateWebService.CreateWeb(txtSiteCollPath.Text, txtWebName.Text, txtTitle.Text, txtDescription.Text, ddlTemplate.SelectedValue);

                // Step 6: Display success message
                lblMessage.Text = ("Successfuly added new site \'" + txtWebName.Text + "\'");
                lblMessage.Visible = true;
            }
            else
            {

                // Step 3: Prompt user to enter all data
                lblMessage.Text = "Please fill in all fields";
                lblMessage.Visible = true;
            }
        }
        catch (Exception ex)
        {
            // Step 7: Display error message
            lblMessage.Text = ex.Message;
            lblMessage.Visible = true;
        }
    }
}
