using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
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

    // Get a sorted list of available list templates
    protected void cmdLookupListTemplates_Click(object sender, EventArgs e)
    {
        WebsService.Webs objWebs = new WebsService.Webs();
        objWebs.Url = txtSiteUrl.Text + "/" + txtWebName.Text + "/_vti_bin/Webs.asmx";
        objWebs.Credentials = System.Net.CredentialCache.DefaultCredentials;

        XmlNode xnListTemplates;
        xnListTemplates = objWebs.GetListTemplates();

        // Get sorted list of available list templates
        ArrayList arrListItems = new ArrayList();
        foreach (XmlNode xnListTemplate in xnListTemplates.ChildNodes)
        {
            try
            {
                if (xnListTemplate.Attributes["Hidden"].Value.ToString() != "TRUE")
                {
                    arrListItems.Add(xnListTemplate.Attributes["DisplayName"].Value + ":" + xnListTemplate.Attributes["Type"].Value);
                }
            }
            catch
            {
                arrListItems.Add(xnListTemplate.Attributes["DisplayName"].Value + ":" + xnListTemplate.Attributes["Type"].Value);
            }
            
        }
        arrListItems.Sort();

        // Add them to the drop-down list
        ddlListType.Items.Clear();
        ListItem li;
        foreach (string templateData in arrListItems)
        {
            li = new ListItem(templateData.Split(':')[0], templateData.Split(':')[1]);
            ddlListType.Items.Add(li);
        }
        ddlListType.SelectedIndex = 0;

        // Show the rest of the form
        Panel1.Visible = true;

    }

    // Add the new list
    protected void cmdCreateList_Click(object sender, EventArgs e)
    {
        try
        {
            lblErrorMsg.Visible = false;

            // Step 1: Create an instance of a list service
            ListsService.Lists objLists = new ListsService.Lists();
            objLists.Url = txtSiteUrl.Text + "/" + txtWebName.Text + "/_vti_bin/Lists.asmx";
            objLists.Credentials = System.Net.CredentialCache.DefaultCredentials;

            // Step 2: Create the new list
            int listTemplateType = int.Parse(ddlListType.SelectedValue);
            objLists.AddList(txtListName.Text, "", listTemplateType);

            // Step 3: Add any user defined fields - this requires
            //  a bit of CAML
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode xnNewFields = xmlDoc.CreateNode(XmlNodeType.Element,"Fields","");

            if (txtUDFName1.Text != "")
            {
                xnNewFields.InnerXml +=
                    "<Method ID='1'>" +
                    "<Field Type='" + ddlUDFType1.SelectedValue + "' DisplayName='" + txtUDFName1.Text + "'/>" +
                    "</Method>";
            }
            if (txtUDFName2.Text != "")
            {
                xnNewFields.InnerXml +=
                    "<Method ID='2'>" +
                    "<Field Type='" + ddlUDFType2.SelectedValue + "' DisplayName='" + txtUDFName2.Text + "'/>" +
                    "</Method>";
            }
            if (txtUDFName3.Text != "")
            {
                xnNewFields.InnerXml +=
                    "<Method ID='3'>" +
                    "<Field Type='" + ddlUDFType3.SelectedValue + "' DisplayName='" + txtUDFName3.Text + "'/>" +
                    "</Method>";
            }
            // We can pass "null" values for any parameters we don't need to change,
            // so we're only passing data for the new fields we want to add
            objLists.UpdateList(txtListName.Text,null,xnNewFields,null,null,null);

            // Step 4: Add any new fields to the default view
            ViewsService.Views objViews = new ViewsService.Views();
            objViews.Url = txtSiteUrl.Text + "/" + txtWebName.Text + "/_vti_bin/Views.asmx";
            objViews.Credentials = System.Net.CredentialCache.DefaultCredentials;

            // Get a handle to the view
            XmlNode xnDefaultView = objViews.GetView(txtListName.Text, "");

            // Get the GUID of the view, which we'll need when we call the
            // UpdateView() method below
            string viewName = xnDefaultView.Attributes["Name"].Value;

            // Get any existing fields in the view, so we can add the new fields
            // to that list.  To do this we need to find the "ViewFields"
            // node (if one exists), and grab it's XML to use as a starting point.
            XmlNode xnViewFields = xmlDoc.CreateNode(XmlNodeType.Element, "ViewFields", "");
            foreach (XmlNode childNode in xnDefaultView.ChildNodes)
            {
                if (childNode.Name == "ViewFields")
                {
                    xnViewFields.InnerXml += childNode.InnerXml;
                }
            }

            // Now add the new fields to end of the list of pre-existing
            // view fields.
            if (txtUDFName1.Text != "")
                xnViewFields.InnerXml += "<FieldRef Name='" + txtUDFName1.Text + "'/>";
            if (txtUDFName2.Text != "")
                xnViewFields.InnerXml += "<FieldRef Name='" + txtUDFName2.Text + "'/>";
            if (txtUDFName3.Text != "")
                xnViewFields.InnerXml += "<FieldRef Name='" + txtUDFName3.Text + "'/>";

            // Update the view.  As with the ListUpdate() method, we only need to pass
            // parameters for data we want to change.  We can pass "null" values for
            // all the rest.
            objViews.UpdateView(txtListName.Text, viewName, null, null, xnViewFields, null, null, null);

            // Step 5: If requested, open new list
            if (cbOpenList.Checked)
            {
                Response.Redirect(txtSiteUrl.Text.ToString() + "/" + 
                    txtWebName.Text.ToString() + "/lists/" + 
                    txtListName.Text.ToString());
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

    // Set standard type values for UDF type ddl's
    private void InitializeTypeDDL(ref DropDownList ddl)
    {
        ddl.Items.Clear();
        ddl.Items.Add("DateTime");
        ddl.Items.Add("Number");
        ddl.Items.Add("Text");
        ddl.SelectedIndex = 2;
    }
}
