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

public partial class _Default : System.Web.UI.Page 
{

    const string COMMENTS_SITE_URL = "http://mgerow-moss-vpc";
    const string COMMENTS_WEB_NAME = "DOCS";
    const string COMMENTS_LIST = "DocComments";

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // Step 2: Get parse querystring for parameters.
            // We are assuming that this page is hosted in
            // a page viewer web part on a web part page.
            // So get the parameters passed to the web-part
            // page contining this page in a PageViewer
            string[] querystring = 
                Server.UrlDecode(
                    Request.UrlReferrer.Query).ToLower().Replace("?", "").Split('&');

            // Step 3: Display querystring parameters
            for (int i = 0; i < querystring.Length; i++)
            {
                if (querystring[i].IndexOf("itemid=") != -1) 
                    lblItemId.Text = querystring[i].Split('=')[1];
                if (querystring[i].IndexOf("itemurl=") != -1)
                {
                    hlItemUrl.NavigateUrl = querystring[i].Split('=')[1];
                    hlItemUrl.Text = querystring[i].Split('=')[1];
                }
                if (querystring[i].IndexOf("listid=") != -1) 
                    lblListId.Text = querystring[i].Split('=')[1];
                if (querystring[i].IndexOf("siteurl=") != -1)
                {
                    hlSiteUrl.NavigateUrl = querystring[i].Split('=')[1];
                    hlSiteUrl.Text = querystring[i].Split('=')[1];
                }
            }

            // Step 4: Get the list name and url from its GUID
            SPSite site = new SPSite(hlSiteUrl.NavigateUrl);
            SPWeb web = site.OpenWeb(hlSiteUrl.NavigateUrl.ToLower().Replace(site.Url, ""));
            Guid guid = new Guid(lblListId.Text);
            SPList origList = web.Lists[guid];
            lblListName.Text = origList.Title;
            hlListUrl.NavigateUrl = web.Url + "/" + origList.RootFolder.Url;
            hlListUrl.Text = web.Url + "/" + origList.RootFolder.Url;

            // Step 5: Display existing comments for this document
            SPSite siteComments = new SPSite(COMMENTS_SITE_URL);
            SPWeb webComments = siteComments.OpenWeb(COMMENTS_WEB_NAME);
            SPList docComments = webComments.Lists[COMMENTS_LIST];
            DataTable dtComments = docComments.Items.GetDataTable();
            dtComments.TableName = "Comments";
            DataView dvComments = new DataView(
                dtComments, "ItemUrl='" 
                    + hlItemUrl.NavigateUrl 
                    + "'", "Created DESC", 
                DataViewRowState.CurrentRows);
            GridView1.DataSource = dvComments;
            GridView1.DataBind();
        }
        catch (Exception ex)
        {
        }
    }
    protected void cmdSave_Click(object sender, EventArgs e)
    {
        // Get handle to web
        SPSite siteComments = new SPSite(COMMENTS_SITE_URL);
        SPWeb webComments = siteComments.OpenWeb(COMMENTS_WEB_NAME);
        webComments.AllowUnsafeUpdates = true;

        // Step 6: Write new comment to DOCCOMMENTS list
        SPList docComments = webComments.Lists[COMMENTS_LIST];
        SPListItem item = docComments.Items.Add();
        item["ItemId"] = lblItemId.Text;
        item["ItemUrl"] = hlItemUrl.NavigateUrl;
        item["ListId"] = lblListId.Text;
        item["SiteUrl"] = hlSiteUrl.NavigateUrl;
        item["Comment"] = txtComments.Text;
        item.Update();

        // Step 7: Return user to list
        returnToList();
    }
    protected void cmdCancel_Click(object sender, EventArgs e)
    {
        returnToList();
    }

    private void returnToList()
    {
        // Because this page is running in a page viewer
        // (i.e. in an <IFRAME>), a redirect statement would
        // display the target url in the frame, whereas we
        // want the target page displayed at the top-level.
        // To accomplish this we'll insert a bit of JavaScript
        // to perform a window.open().
        Response.Write("<script>window.open('" + hlListUrl.NavigateUrl + "','_top');</script>");
    }
}
