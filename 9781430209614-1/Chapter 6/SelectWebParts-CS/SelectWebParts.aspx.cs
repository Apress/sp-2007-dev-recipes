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
using Microsoft.SharePoint.WebPartPages;
using System.Xml;

public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    private static Microsoft.SharePoint.WebPartPages.ContentEditorWebPart
    AddContentEditorWebPart(
        string strContent,
        string strTitle,
        string strSiteUrl,
        string strWebName,
        string strDocLibName,
        string strPage,
        string strZone,
        int numOrder, System.Web.UI.WebControls.WebParts.PersonalizationScope pScope)
        {
            try
            {
                // Create an empty content editor web part. 
                Microsoft.SharePoint.WebPartPages.ContentEditorWebPart cewp
                    = new Microsoft.SharePoint.WebPartPages.ContentEditorWebPart();

                // Create an xml element object and transfer the content 
                //into the web part. 
                XmlDocument xmlDoc = new XmlDocument();
                System.Xml.XmlElement xmlElem = xmlDoc.CreateElement("xmlElem");
                xmlElem.InnerText = strContent;
                cewp.Content = xmlElem;

                // Call generic method to add the web part 
                cewp = (Microsoft.SharePoint.WebPartPages.ContentEditorWebPart)
                AddWebPart(
                    cewp,
                    strTitle,
                    strSiteUrl,
                    strWebName,
                    strDocLibName,
                    strPage,
                    strZone,
                    numOrder,
                    System.Web.UI.WebControls.WebParts.PersonalizationScope.Shared);

                return cewp;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "AddContentEditorWebPart() error: " + ex.Message);
            }
        }

    private static System.Web.UI.WebControls.WebParts.WebPart
    AddWebPart(
    System.Web.UI.WebControls.WebParts.WebPart oWebPart,
    string strTitle,
    string strSiteUrl,
    string strWebName,
    string strDocLibName,
    string strPage,
    string strZone,
    int numOrder, System.Web.UI.WebControls.WebParts.PersonalizationScope pScope)
        {
            try
            {
                // Get handles to site, web and page to which 
                // web part will be added. 
                SPSite site = new SPSite(strSiteUrl);
                SPWeb web = site.OpenWeb(strWebName);

                // Enable update of page 
                web.AllowUnsafeUpdates = true;
                SPLimitedWebPartManager webParts;
                if ((strDocLibName != ""))
                {
                    webParts = web.GetLimitedWebPartManager(
                        strDocLibName + "/" + strPage, pScope);
                }
                else
                {
                    webParts = web.GetLimitedWebPartManager(strPage, pScope);
                }

                // If web part page is in a document library, 
                // disable checkout requirement 
                // for duration of update
                SPList list = null;
                bool origForceCheckoutValue = false;
                if ((strDocLibName != ""))
                {
                    list = web.Lists[strDocLibName];
                    origForceCheckoutValue = list.ForceCheckout;
                    list.ForceCheckout = false;
                    list.Update();
                }

                // Add the web part 
                oWebPart.Title = strTitle;
                webParts.AddWebPart(oWebPart, strZone, numOrder);

                // Save changes back to the SharePoint database 
                webParts.SaveChanges(oWebPart);
                web.Update();

                // If necessary, restore ForceCheckout setting
                if ((strDocLibName != ""))
                {
                    list.ForceCheckout = origForceCheckoutValue;
                    list.Update();
                }
                return oWebPart;
            }
            catch (Exception ex)
            {
                throw new Exception(("AddWebPart() error: " + ex.Message));
            }
        }

    protected void Finish_Click(object sender, EventArgs e)
    {
        // Step 1: Get handle to web site being created
        SPWeb web = SPControl.GetContextWeb(Context);
        web.AllowUnsafeUpdates = true; 

        // Step 2: If requested, cleare out any existing
        // web parts
        if (cbRemoveExisting.Checked)
        {
            SPLimitedWebPartManager webparts
                = web.GetLimitedWebPartManager("default.aspx", PersonalizationScope.Shared);
            for (int i = webparts.WebParts.Count - 1; i > -1; i--)
            {
                webparts.DeleteWebPart(webparts.WebParts[i]);
            }
        }

        // Step 3: If requested, add an instance of a SharePoint 
        // "ContentEditorWebPart", which is a descendent of 
        // the generic .NET 2.0 WebPart class
        if (cbCEWP.Checked)
        {
            Microsoft.SharePoint.WebPartPages.ContentEditorWebPart oCEwp
                = new Microsoft.SharePoint.WebPartPages.ContentEditorWebPart();
            oCEwp = AddContentEditorWebPart(
                "Hello World!",
                "Hello World web part",
                web.Site.Url.ToString(),
                web.ServerRelativeUrl.Substring(1),
                "",
                "Default.aspx",
                "Right",
                0,
                System.Web.UI.WebControls.WebParts.PersonalizationScope.Shared);
        }

        // Step 4: If requested, add a PageViewer web part
        if (cbPVWP.Checked)
        {
            Microsoft.SharePoint.WebPartPages.PageViewerWebPart oPVwp
                = new Microsoft.SharePoint.WebPartPages.PageViewerWebPart();
            oPVwp.SourceType = PathPattern.URL;
            oPVwp.ContentLink = txtPVUrl.Text;
            oPVwp.Height = "1000px";
            oPVwp = (Microsoft.SharePoint.WebPartPages.PageViewerWebPart)
            AddWebPart(
                    oPVwp,
                    "And here's my page viewer web part!",
                    web.Site.Url.ToString(),
                    web.ServerRelativeUrl.Substring(1),
                    "",
                    "Default.aspx",
                    "Left",
                    0,
                System.Web.UI.WebControls.WebParts.PersonalizationScope.Shared);
        }

        // Step 5: Now take the user to the home page of the new
        // web site
        Response.Redirect(web.ServerRelativeUrl);
    }

    // Determine whether Url textbox should
    // be enabled based on whether 
    // option to add a Page Viewer web part
    // is checked
    protected void cbPVWP_CheckedChanged(object sender, EventArgs e)
    {
        txtPVUrl.Enabled = cbPVWP.Checked;
    }
}
