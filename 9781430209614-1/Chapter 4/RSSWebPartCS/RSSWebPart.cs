using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Data;

namespace RSSWebPartCS
{
    public class RSSWebPart : WebPart
    {
        // Local variables to hold web part
        // property values
        string _url;
        bool _newPage = true;
        bool _showDescription = true;
        bool _showUrl = true;

        // Property to determine whether article should
        // be opened in same or new page
        [Personalizable]
        [WebBrowsable]
        public bool NewPage
        {
            get
            {
                return _newPage;
            }
            set
            {
                _newPage = value;
            }
        }

        // Should Description be displayed?
        [Personalizable]
        [WebBrowsable]
        public bool ShowDescription
        {
            get
            {
                return _showDescription;
            }
            set
            {
                _showDescription = value;
            }
        }

        // Should Url be displayed?
        [Personalizable]
        [WebBrowsable]
        public bool ShowUrl
        {
            get
            {
                return _showUrl;
            }
            set
            {
                _showUrl = value;
            }
        }

        // Property to set Url of RSS feed
        [Personalizable]
        [WebBrowsable]
        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                _url = value;
            }
        }

        // This is where the HTML gets rendered to the
        // web part page.
        protected override void RenderContents(HtmlTextWriter writer)
        {
            base.RenderContents(writer);

            // Step 1: Ensure Url property has been provided
            if (Url != "")
            {
                // Display heading with RSS location url
                if (ShowUrl)
                {
                    writer.WriteLine("<hr/>");
                    writer.WriteLine("<span style='font-size: larger;'>");
                    writer.WriteLine("Results for: ");
                    writer.WriteLine("<strong>");
                    writer.WriteLine(Url);
                    writer.WriteLine("</strong>");
                    writer.WriteLine("</span>");
                    writer.WriteLine("<hr/>");
                }
                displayRSSFeed(writer);
            }
            else
            {
                // Tell user they need to fill in the Url property
                writer.WriteLine("<font color='red'>RSS Url cannot be blank</font>");
            }
        }

        private void displayRSSFeed(HtmlTextWriter writer)
        {
            try
            {
                // Step 2: Read the RSS feed into memory
                System.Net.WebRequest wReq;
                wReq = System.Net.WebRequest.Create(Url);
                wReq.Credentials = System.Net.CredentialCache.DefaultCredentials;

                // Return the response. 
                System.Net.WebResponse wResp = wReq.GetResponse();
                System.IO.Stream respStream = wResp.GetResponseStream();

                // Load RSS stream into a data set for easier processing
                DataSet dsXML = new DataSet();
                dsXML.ReadXml(respStream);

                // Step 4: Loop through all items returned, displaying results
                string target = "";
                if (NewPage) 
                    target = "target='_new'";

                foreach (DataRow item in dsXML.Tables["item"].Rows)
                {
                    // Step 5: Write the title, link and description to page
                    writer.WriteLine(
                        "<a href='" + item["link"] + "' " + target + ">" + item["title"] + "</a>" +
                        "<br/>" +
                        "<span style='color:silver'>" + item["pubDate"] + "</span>" +
                        "<br/>");

                    if (ShowDescription)
                        writer.WriteLine("<br/>" + item["description"]); 
                    
                    writer.WriteLine("<hr/>");
                }
            }
            catch (Exception ex)
            {
                // Step 3: If error occurs, notify end-user
                writer.WriteLine(
                    "<font color='red'><strong>" +
                    "An error occurred while attempting to process the selected RSS feed. " +
                    "Please verify that the url provided references a valid RSS page."
                );
            }
        }
        
    }
}
