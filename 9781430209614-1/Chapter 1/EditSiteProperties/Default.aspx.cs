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
using Microsoft.SharePoint.WebControls;
using System.Collections;

public partial class _Default : System.Web.UI.Page 
{
    
    // Hard-coded to save/retrieve properties of root site collection
    SPSite site = new SPSite("http://localhost");

    protected void Page_Load(object sender, EventArgs e)
    {
     if (!IsPostBack)
        {
            SPWeb web = site.RootWeb;
            
            // Step 1: Determine whether a value exists for customer id key
            if (web.Properties.ContainsKey("mg_CustomerId"))
            {
                // Step 2: Initialize to stored value
                txtCustomerId.Text = web.Properties["mg_CustomerId"].ToString();
            }
            else
            {
                // Step 3: Otherwise set to empty string
                txtCustomerId.Text = "";
            }

            // Step 4: Determine whether a value exists for bill flag
            if (web.Properties.ContainsKey("mg_BillCustomer"))
            {
                // Step 5: Initialize to stored value
                cbBillCustomer.Checked = (web.Properties["mg_BillCustomer"].ToString().ToLower() == "true");
            }
            else
            {
                // Step 6: Otherwise set default value
                cbBillCustomer.Checked = true;
            }
         // Display list of key/value pairs
         refreshPropertyList();
        }
    }

    protected void refreshPropertyList()
    {
        SPWeb web = site.RootWeb;
        DataTable dtProperties = new DataTable("PropertyList");
        DataView dvProperties = new DataView();
        DataRow drProperty;
        dtProperties.Columns.Add("Key");
        dtProperties.Columns.Add("Value");
        
        // Step through list of properties, adding
        // key/value pairs to data table.
        foreach (object key in web.Properties.Keys)
        {
            drProperty = dtProperties.NewRow();
            drProperty["Key"] = key.ToString();
            drProperty["Value"] = web.Properties[key.ToString()];
            dtProperties.Rows.Add(drProperty);
            dtProperties.AcceptChanges();
        }

        // Sort the list and display
        dvProperties.Table = dtProperties;
        dvProperties.Sort = "Key";
        gvPropertyList.DataSource = dvProperties;
        gvPropertyList.DataBind();
    }
    
    protected void cmdSave_Click(object sender, EventArgs e)
    {
        SPWeb web = site.RootWeb;

        // Error will occur if AllowUnsafeUpdates property
        // not set prior to update
        web.AllowUnsafeUpdates = true;
        web.Properties["mg_CustomerId"] = txtCustomerId.Text;
        web.Properties["mg_BillCustomer"] = cbBillCustomer.Checked.ToString();
        web.Properties.Update();

        // Redisplay list to reflext changes
        refreshPropertyList();
    }
}
