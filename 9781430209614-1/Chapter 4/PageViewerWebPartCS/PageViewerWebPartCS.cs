using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Data;

namespace PageViewerWebPartCS
{
    public class PageViewerWebPartCS : WebPart
    {
        // Local variable to hold property values
        string _url = "";
        bool _impersonate = true;
        string _domain = "";
        string _user = "";
        string _password = "";
        bool _debug = false;

        // Property to set Url of page to display
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

        // If explicit credentials have been requested
        // the following three properties will
        // will be used to construct the credentials
        // to pass to the page
        [Personalizable]
        [WebBrowsable]
        public string Domain
        {
            get
            {
                return _domain;
            }
            set
            {
                _domain = value;
            }
        }

        [Personalizable]
        [WebBrowsable]
        public string User
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
            }
        }

        [Personalizable]
        [WebBrowsable]
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
            }
        }

        // Should user be impersonated?
        [Personalizable]
        [WebBrowsable]
        public bool Impersonate
        {
            get
            {
                return _impersonate;
            }
            set
            {
                _impersonate = value;
            }
        }

        // Display debug info?
        [Personalizable]
        [WebBrowsable]
        public bool Debug
        {
            get
            {
                return _debug;
            }
            set
            {
                _debug = value;
            }
        }

        // This is where the HTML gets rendered to the
        // web part page.
        protected override void RenderContents(HtmlTextWriter writer)
        {
            base.RenderContents(writer);

            // Step 1: If debug info requested, display it
            if (Debug)
            {
                writer.WriteLine("Url: " + Url + "<br/>");
                writer.WriteLine("Impersonate: " + Impersonate.ToString() + "<br/>");
                writer.WriteLine("Domain: " + Domain + "<br/>");
                writer.WriteLine("User: " + User + "<br/>");
                writer.WriteLine("Password: " + Password + "<br/>");
                writer.WriteLine("<hr/>");
            }

            // Step 2: Make sure Url is provided
            if (Url == "")
            {
                writer.WriteLine("<font color='red'>Please enter a valid Url</font>");
                return;
            }

            // Step 3: Create a web request to read desired page
            System.Net.WebRequest wReq;
            wReq = System.Net.WebRequest.Create(Url);

            // Step 4: Set the security as appropriate
            if (Impersonate)
            {
                wReq.Credentials = System.Net.CredentialCache.DefaultCredentials;
            }
            else
            {
                wReq.Credentials = new System.Net.NetworkCredential(User, Password, Domain);
            }

            // Step 5: Get the page contents as a string variable
            try
            {
                System.Net.WebResponse wResp = wReq.GetResponse();
                System.IO.Stream respStream = wResp.GetResponseStream();
                System.IO.StreamReader respStreamReader = new System.IO.StreamReader(respStream, System.Text.Encoding.ASCII);
                string strHTML = respStreamReader.ReadToEnd();

                // Step 6: Render the HTML to the web part page
                writer.Write(strHTML);
            }
            catch (Exception e)
            {
                writer.Write("<font color='red'>" + e.Message);
            }
        }
    }
}
