using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.StsAdmin;
using System.Xml;
using Microsoft.SharePoint.WebPartPages;
using System.Web;
using System.Web.UI.WebControls;

namespace CustomizeStsAdm
{
    // Step 2: Class must implement the ISPStsadmCommand 
    // interface to allow it to be registered with 
    // STSADM command
    public class MyCustomizations : ISPStsadmCommand
    {

        // Step 3: implement the GetHelpMessage method
        //  to provide a response when user runs the
        //  new command with the -help flag
        public string GetHelpMessage(string command)
        {
            command = command.ToLower();

            // Step 4: Provide as many cases as there are
            // commands supported by this class
            switch (command)
            {
                case "adddoclibtosite":
                    return "\n-site <full url to a site collection in SharePoint>"
                        + "\n-web <web name to add doclib to"
                        + "\n-libname <name to give new doclib>"
                        + "\n-description <description for new doclib>";

                case "addnoticetosite":
                    return "\n-site <full url to a site collection in SharePoint>"
                        + "\n-web <web name to add doclib to"
                        + "\n-text <text of notice to add>";

                default:
                    throw new InvalidOperationException();
            }
        }

        // Step 5: implement the Run method to
        //  actually execute the commands defined in this
        //  class
        public int Run(string command, StringDictionary keyValues, out string output)
        {
            command = command.ToLower();

            // Step 6: provide as many cases as there are
            //  commands supported by this class
            switch (command)
            {
                case "adddoclibtosite":
                    return addDoclibToSite(keyValues, out output);

                case "addnoticetosite":
                    return addNoticeToSite(keyValues, out output);

                default:
                    throw new InvalidOperationException();
            }
        }

        // Step 7: add the methods and code that will perfom
        //  the operations for each command supported
        private int addNoticeToSite(StringDictionary keyValues, out string output)
        {
            // Get handle to target web site
            SPSite site = new SPSite(keyValues["site"]);

            // Get handle to web
            SPWeb web;
            if (keyValues["web"] == null)
            {
                web = site.RootWeb;
            }
            else
            {
                web = site.OpenWeb(keyValues["web"]);
            }

            // Get a handle to the default page of the target web
            // NOTE: we're assuming page called "default.aspx" exists
            web.AllowUnsafeUpdates = true;
            SPLimitedWebPartManager webParts = web.GetLimitedWebPartManager("default.aspx", System.Web.UI.WebControls.WebParts.PersonalizationScope.Shared);

            // Create XML element to hold notice text
            ContentEditorWebPart cewp = new ContentEditorWebPart();
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement xmlElem = xmlDoc.CreateElement("xmlElem");
            xmlElem.InnerXml =
                "<![CDATA["
                + "<div align='center' "
                + "   style='font-family: Arial; font-size: medium; font-weight: bold; "
                + "   background-color: red; color: white; border: medium solid black; "
                + "   width: 50%; padding: 10px; margin: 10px;'>"
                + keyValues["text"].ToString()
                + "</div>"
                + "]]>";


            // Add the CEWP to the page
            // NOTE: we're assuming a zone called "Left" exists
            cewp.ChromeType = System.Web.UI.WebControls.WebParts.PartChromeType.None;
            cewp.Content = xmlElem;
            webParts.AddWebPart(cewp, "Left", 0);
            webParts.SaveChanges(cewp);
            web.Update();

            output = "";
            return 0;
        }

        private int addDoclibToSite(StringDictionary keyValues, out string output)
        {
            // Get handle to target web site
            SPSite site = new SPSite(keyValues["site"]);

            // Get handle to web
            SPWeb web;
            if (keyValues["web"] == null)
            {
                web = site.RootWeb;
            }
            else
            {
                web = site.OpenWeb(keyValues["web"]);
            }

            // Add a list to target site
            web.AllowUnsafeUpdates = true;
            web.Lists.Add(keyValues["listname"], keyValues["description"], SPListTemplateType.DocumentLibrary);
            SPList list = web.Lists[keyValues["libname"]];
            list.OnQuickLaunch = true;
            list.Update();
            web.Update();

            output = "";
            return 0;
        }
    }
}
