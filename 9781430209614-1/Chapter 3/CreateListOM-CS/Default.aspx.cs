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
        if (!IsPostBack)
        {
            // Get column types
            InitializeTypeDDL(ref ddlUDFType1);
            InitializeTypeDDL(ref ddlUDFType2);
            InitializeTypeDDL(ref ddlUDFType3);
        }

    }
    protected void cmdCreateList_Click(object sender, EventArgs e)
    {
        try
        {
            lblErrorMsg.Visible = false;

            // Step 1: get a handle to the site collection and web site
            SPSite site = new SPSite(txtSiteUrl.Text);
            SPWeb web = site.AllWebs[txtWebName.Text];
            SPListCollection listCollection = web.Lists;
            web.AllowUnsafeUpdates = true;

            // Step 2: Create the new list
            listCollection.Add(txtListName.Text, "", web.ListTemplates[ddlListType.SelectedItem.Text]);
            SPList newList = listCollection[txtListName.Text];

            // Step 3: Add any user defined fields
            if (txtUDFName1.Text != "") 
                AddField(newList, txtUDFName1, ddlUDFType1);
            if (txtUDFName2.Text != "") 
                AddField(newList, txtUDFName2, ddlUDFType2);
            if (txtUDFName3.Text != "") 
                AddField(newList, txtUDFName3, ddlUDFType3);

            // Step 4: Save the changes
            newList.Update();
            web.Update();

            // Step 5: If requested, open new list
            if (cbOpenList.Checked)
            {
                Response.Redirect(web.Url.ToString() + "/lists/" + newList.Title.ToString());
            }
            else 
            {
                // Step 6: Display success message
                this.RegisterClientScriptBlock("Success", "<script>alert ('List successfuly added');</script>");
            }
        }
        catch (Exception ex)
        {
            lblErrorMsg.Text = ex.Message;
            lblErrorMsg.Visible = true;
        }
    }

    // Add the UDF to list and default view
    private void AddField(SPList newList, TextBox tb, DropDownList ddl)
    {
        SPView defaultView = newList.DefaultView;
        newList.Fields.Add(tb.Text, GetFieldType(ddl), false);
        SPField newField = newList.Fields[tb.Text];
        defaultView.ViewFields.Add(newField);
        defaultView.Update();
    }

    // Return SP field type from ddl value for UDF type
    private SPFieldType GetFieldType(DropDownList ddlUDFType)
    {
        switch (ddlUDFType.SelectedItem.Value)
        {
            case ("Number"):
                return SPFieldType.Number;
            case ("Text"):
                return SPFieldType.Text;
            case ("Date"):
                return SPFieldType.DateTime;
            default:
                return SPFieldType.Text;
        }
    }

    // Get a sorted list of all templates available
    protected void cmdLookupListTemplates_Click(object sender, EventArgs e)
    {
        try
        {
            lblErrorMsg.Visible = false;

            SPSite site = new SPSite(txtSiteUrl.Text);
            SPWeb web = site.AllWebs[txtWebName.Text];

            // Get sorted list of available list templates
            ArrayList arrListItems = new ArrayList();
            foreach (SPListTemplate listTemplate in web.ListTemplates)
            {
                if (!listTemplate.Hidden)
                {
                   arrListItems.Add(listTemplate.Name);
                }
            }
            arrListItems.Sort();

            // Add them to the drop-down list
            ddlListType.Items.Clear();
            ListItem li;
            foreach (string templateName in arrListItems)
            {
                ddlListType.Items.Add(templateName);
            }
            ddlListType.SelectedIndex = 0;

            // Show the rest of the form
            Panel1.Visible = true;

        }
        catch (Exception ex)
        {
            lblErrorMsg.Text = ex.Message;
            lblErrorMsg.Visible = true;
        }
    }

    // Set standard type values for UDF type ddl's
    private void InitializeTypeDDL(ref DropDownList ddl)
    {
        ddl.Items.Clear();
        ddl.Items.Add("Date");
        ddl.Items.Add("Number");
        ddl.Items.Add("Text");
        ddl.SelectedIndex = 2;
    }
}
