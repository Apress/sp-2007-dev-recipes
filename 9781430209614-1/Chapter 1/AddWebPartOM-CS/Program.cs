using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using System;
using System.Web;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.WebControls;
using System.Xml;

class Module1
{

    static void Main()
    {
        
        // Example 1:
        // Add an instance of a SharePoint "ContentEditorWebPart", which
        // is a descendent of the generic .NET 2.0 WebPart class
        Microsoft.SharePoint.WebPartPages.ContentEditorWebPart oCEwp = new Microsoft.SharePoint.WebPartPages.ContentEditorWebPart();
        oCEwp = AddContentEditorWebPart( 
            "Hello World!", 
            "Hello World web part", 
            "http://localhost", 
            "BBB", 
            "", 
            "Default.aspx", 
            "Right", 
            0, 
            System.Web.UI.WebControls.WebParts.PersonalizationScope.Shared);
        
        // Example 2:
        // Add a PageViewer web part
        Microsoft.SharePoint.WebPartPages.PageViewerWebPart oPVwp = new Microsoft.SharePoint.WebPartPages.PageViewerWebPart();
        oPVwp.SourceType = PathPattern.URL;
        oPVwp.ContentLink = "http://www.yahoo.com";
        oPVwp = (Microsoft.SharePoint.WebPartPages.PageViewerWebPart) AddWebPart(
            oPVwp, 
            "Hello World web part", 
            "http://localhost", 
            "BBB", 
            "", 
            "Default.aspx", 
            "Left", 
            0, 
            System.Web.UI.WebControls.WebParts.PersonalizationScope.Shared);

    }

    private static Microsoft.SharePoint.WebPartPages.ContentEditorWebPart AddContentEditorWebPart(string strContent, string strTitle, string strSiteUrl, string strWebName, string strDocLibName, string strPage, string strZone, int numOrder, System.Web.UI.WebControls.WebParts.PersonalizationScope pScope)
    {
        try
        {
            // Create an empty content editor web part. 
            Microsoft.SharePoint.WebPartPages.ContentEditorWebPart cewp = new Microsoft.SharePoint.WebPartPages.ContentEditorWebPart();
            
            // Create an xml element object and transfer the content into the web part. 
            XmlDocument xmlDoc = new XmlDocument();
            System.Xml.XmlElement xmlElem = xmlDoc.CreateElement("xmlElem");
            xmlElem.InnerText = strContent;
            cewp.Content = xmlElem;
            
            // Call generic method to add the web part 
            cewp =  (Microsoft.SharePoint.WebPartPages.ContentEditorWebPart) AddWebPart ( 
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
            throw new Exception(("AddContentEditorWebPart() error: " + ex.Message));
        }
    }

    private static System.Web.UI.WebControls.WebParts.WebPart AddWebPart(System.Web.UI.WebControls.WebParts.WebPart oWebPart, string strTitle, string strSiteUrl, string strWebName, string strDocLibName, string strPage, string strZone, int numOrder, System.Web.UI.WebControls.WebParts.PersonalizationScope pScope)
    {
        try
        {
            // Get handles to site, web and page to which web part will be added. 
            SPSite site = new SPSite(strSiteUrl);
            SPWeb web = site.OpenWeb(strWebName);
            
            // Enable update of page 
            web.AllowUnsafeUpdates = true;
            SPLimitedWebPartManager webParts;
            if ((strDocLibName != ""))
            {
                webParts = web.GetLimitedWebPartManager((strDocLibName + ("/" + strPage)), pScope);
            }
            else
            {
                webParts = web.GetLimitedWebPartManager(strPage, pScope);
            }
            
            // If web part page is in a document library, disable checkout requirement 
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
}