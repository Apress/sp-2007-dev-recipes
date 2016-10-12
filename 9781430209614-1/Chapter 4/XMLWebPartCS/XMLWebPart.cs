using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Data;

namespace XMLWebPartCS
{
    public class XMLWebPart : WebPart
    {
        // Local variable to hold property values
        string _url = "";
        bool _impersonate = true;
        string _domain = "";
        string _user = "";
        string _password = "";
        bool _debug = false;
        enumFormatUsing _formatUsing = enumFormatUsing.DataGrid;
        string _xsltPath = "";

        //ENUM types will result in drop-down lists in 
        //the webpart property sheet 
        public enum enumFormatUsing
        {
            DataGrid = 1,
            XSLT = 2
        }

        // Property to set Url of source XML document
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("Url of XML document")]
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

        //Create property to determine whether datagrid or 
        //XSLT should be used to format output 
        [Personalizable(PersonalizationScope.Shared), WebBrowsable(), 
            WebDisplayName("Format Using:"),
            WebDescription("What method do you want " + "to use to format the results.")]
        public enumFormatUsing FormatUsing
        {
            get { return _formatUsing; }

            set { _formatUsing = value; }
        }

        //If XSLT will be used, this property specifies 
        //its server-relative path 
        [Personalizable(PersonalizationScope.Shared), 
            WebBrowsable(), WebDisplayName("XSLT Path:"), 
            WebDescription("If formatting with XSLT, " + "provide full path to XSLT document.")]
        public string XSLTPath
        {
            get { return _xsltPath; }

            set { _xsltPath = value; }
        }

        // If explicit credentials have been requested
        // the following three properties, Domain, User, and
        // Password will be used to construct the credentials
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

        // If this option is checked, the web part will use
        // the default credentials of the user viewing
        // the web part page.
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

            // Step 1: Ensure Url property has been provided
            if (Url != "")
            {
                // Step 2: If debug info requested, display it
                if (Debug)
                {
                    writer.WriteLine("Url: " + Url + "<br/>");
                    writer.WriteLine("Impersonate: " + Impersonate.ToString() + "<br/>");
                    writer.WriteLine("Domain: " + Domain + "<br/>");
                    writer.WriteLine("User: " + User + "<br/>");
                    writer.WriteLine("Password: " + Password + "<br/>");
                    writer.WriteLine("Format using: " + FormatUsing + "<br/>");
                    writer.WriteLine("<hr/>");
                }

                // Call helper function to render data as HTML to page
                displayXML(writer);
            }
            else
            {
                // Step 3: Tell user they need to fill in the Url property
                writer.WriteLine("<font color='red'>Source XML url cannot be blank</font>");
            }
        }

        private void displayXML(HtmlTextWriter writer)
        {
            try
            {
                // Step 4: Read the XML document into memory
                System.Net.WebRequest wReq;
                wReq = System.Net.WebRequest.Create(Url);

                // Step 5: Set the security as appropriate
                if (Impersonate)
                {
                    wReq.Credentials = System.Net.CredentialCache.DefaultCredentials;
                }
                else
                {
                    wReq.Credentials = new System.Net.NetworkCredential(User, Password, Domain);
                }

                wReq.Credentials = System.Net.CredentialCache.DefaultCredentials;

                // Step 6: Return the response. 
                System.Net.WebResponse wResp = wReq.GetResponse();
                System.IO.Stream respStream = wResp.GetResponseStream();

                // Step 7: Load XML stream into a data set for easier processing
                DataSet dsXML = new DataSet();
                dsXML.ReadXml(respStream);

                // Step 8: determine display mechanism to use
                if (FormatUsing == enumFormatUsing.DataGrid)
                {
                    // Step 9: Loop through each table in the DataSet,
                    // displaying each in a DataGrid
                    DataGrid dgXML;
                    Label lbl;
                    foreach (DataTable dtXML in dsXML.Tables)
                    {
                        // Display table name
                        lbl = new Label();
                        lbl.Text = "<br/><strong>" + dtXML.TableName.ToUpper() + "</strong><br/><br/>";
                        lbl.RenderControl(writer);

                        // Now display the data
                        dgXML = new DataGrid();
                        dgXML.DataSource = dtXML;
                        dgXML.DataBind();
                        dgXML.RenderControl(writer);
                    }
                }
                else
                {
                    // Step 10: Format using provided XSLT
                    System.Web.UI.WebControls.Xml xml = new System.Web.UI.WebControls.Xml();
                    xml.DocumentContent = dsXML.GetXml();
                    xml.TransformSource = XSLTPath;
                    xml.RenderControl(writer);
                }
            }
            catch (Exception ex)
            {
                // If error occurs, notify end-user
                writer.WriteLine("<font color='red'><strong>" + ex.Message + "</font>");
            }
        }

    }
}
