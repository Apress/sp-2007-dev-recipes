using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Data;
using System.Xml;

namespace SQLWebPartCS
{
    public class SQLWebPartCS : WebPart
    {

        //Define local variables to contain property values 
        string _connectionString = "";
        string _query = "";
        enumFormatUsing _formatUsing = enumFormatUsing.DataGrid;
        string _xsltPath = "";
        bool _includeDebugInfo = false;

        //ENUM types will result in drop-down lists in 
        //the webpart property sheet 
        public enum enumFormatUsing
        {
            DataGrid = 1,
            XSLT = 2
        }

        //Create property to hold SQL connection string 
        [Personalizable(PersonalizationScope.Shared), WebBrowsable(), WebDisplayName("Connection String:"), WebDescription("Connection string to use" + " when connecting to SQL source.")]
        public string ConnectionString
        {
            get { return _connectionString; }

            set { _connectionString = value; }
        }

        //Create property to hold SQL query 
        [Personalizable(PersonalizationScope.Shared), WebBrowsable(), WebDisplayName("SQL Query:"), WebDescription("A valid SQL query to execute.")]
        public string Query
        {
            get { return _query; }

            set { _query = value; }
        }

        //Create property to determine whether datagrid or 
        //XSLT should be used to format output 
        [Personalizable(PersonalizationScope.Shared), WebBrowsable(), WebDisplayName("Format Using:"), WebDescription("What method do you want " + "to use to format the results.")]
        public enumFormatUsing FormatUsing
        {
            get { return _formatUsing; }

            set { _formatUsing = value; }
        }

        //If XSLT will be used, this property specifies 
        //its path 
        [Personalizable(PersonalizationScope.Shared), WebBrowsable(), WebDisplayName("XSLT Path:"), WebDescription("If formatting with XSLT, " + "provide full path to XSLT document.")]
        public string XSLTPath
        {
            get { return _xsltPath; }

            set { _xsltPath = value; }
        }

        //Even though our web parts never have bugs… 
        [Personalizable(PersonalizationScope.Shared), WebBrowsable(), WebDisplayName("Include Debug Info?:"), WebDescription("If selected, will " + "display values of web part properties.")]
        public bool IncludeDebugInfo
        {
            get { return _includeDebugInfo; }

            set { _includeDebugInfo = value; }
        }

        //This is where the real work happens! 
        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {

            //Process any output from the base class first 
            base.RenderContents(writer);

            // Step 1: Display debug info if requested
            if (IncludeDebugInfo)
            {
                writer.Write("Connection String: " + ConnectionString);
                writer.WriteBreak();
                writer.Write("SQL Query: " + Query);
                writer.WriteBreak();
                writer.Write("Format Using: " + FormatUsing.ToString());
                writer.WriteBreak();
                writer.Write("XSLT Path: " + XSLTPath);
                writer.Write("<hr>");
            }

            // Step 2: Query SQL database and return the result set 
            System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(ConnectionString);
            try
            {
                con.Open();
            }
            catch (Exception ex)
            {
                writer.Write("<font color='red'>" + ex.Message + "</font>");
                return;  
            }

            System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(Query, con);
            DataSet ds = new DataSet();

            // Step 3: copy result set to dataset
            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                writer.Write("<font color='red'>" + ex.Message + "</font>");
                return;  
            }

            // Step 4: Format the output using an XSLT or datagrid
            if (FormatUsing == enumFormatUsing.DataGrid)
            {
                // Step 5: format using simple DataGrid
                DataGrid dg = new DataGrid();
                dg.DataSource = ds;
                dg.DataBind();
                dg.RenderControl(writer);
            }
            else
            {
                // Step 6: format using provided XSLT
                System.Web.UI.WebControls.Xml xml = new System.Web.UI.WebControls.Xml();
                xml.DocumentContent = ds.GetXml();
                xml.TransformSource = XSLTPath;
                xml.RenderControl(writer);
            }

        }

    }
}