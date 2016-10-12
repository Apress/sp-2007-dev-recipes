using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Data;

namespace QuerystringWebPartCS
{
    public class QueryStringWebPartCS : WebPart
    {

        // Define local variables
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

        // Display debug info?
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("Debug?")]
        [WebDescription("Check to cause debug information to be displayed")]
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

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            base.RenderContents(writer);

            try
            {
                System.Collections.Specialized.NameValueCollection qs = Page.Request.QueryString;
                if (_debug)
                {
                    // Step 1: Parse the querystring and display
                    if (qs.Count > 0)
                    {
                        writer.Write("<strong>Querystring parameters: </strong>");
                        writer.Write("<blockquote>");
                        for (int i = 0; i < qs.Count; i++)
                        {
                            writer.Write(qs.Keys[i] + " = " + qs[i] + "<br/>");
                        }
                        writer.Write("</blockquote>");
                    }
                    else
                    {
                        writer.Write("No querystring parameters exist<br/>");
                    }
                    // Step 2: Display web part property values
                    writer.Write("<strong>Format output using:</strong> " + _formatUsing + "<br/>");
                    writer.Write("<strong>XSLT path:</strong> " + _xsltPath + "<br/>");
                    writer.Write("<hr/>");
                }

                // Step 3: Display items from Client list based on provided ID
                string clientId = qs["clientId"];
                if (clientId != null)
                {
                    displayClientData(clientId, writer);
                }
                else
                {
                    writer.Write("Client ID was not provided in querystring");
                }
            }
            catch (Exception e)
            {
                writer.Write("<font color='red'>" + e.Message + "</font>");
            }
        }

        private void displayClientData(string clientId, System.Web.UI.HtmlTextWriter writer)
        {
            try
            {
                // Step 4: Get handle to current web site and client list
                SPWeb web = SPControl.GetContextWeb(Context);
                SPList clients = web.Lists["Clients"];

                // Step 5: Copy clients' data into a DataTable object
                // for easier manipulation
                DataSet dsClients = new DataSet("Clients");
                DataTable dtClients = clients.Items.GetDataTable();
                dtClients.TableName = "Clients";

                // Step 6: Filter for the specified client ID
                DataView dvClients = new DataView();
                dvClients.Table = dtClients;
                dvClients.RowFilter = "ClientId = '" + clientId + "'";

                // Step 7: Determine display mechanism to use
                if (FormatUsing == enumFormatUsing.DataGrid)
                {
                    // Step 8: Display as DataGrid
                    DataGrid dgClients = new DataGrid();
                    dgClients.DataSource = dvClients;
                    dgClients.DataBind();
                    dgClients.RenderControl(writer);
                }
                else
                {
                    // Step 9: Format using provided XSLT
                    System.Web.UI.WebControls.Xml xml = new System.Web.UI.WebControls.Xml();
                    dsClients.Tables.Add(dvClients.ToTable("Clients"));
                    xml.DocumentContent = dsClients.GetXml();
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
