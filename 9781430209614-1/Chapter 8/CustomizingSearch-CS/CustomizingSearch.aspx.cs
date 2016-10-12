using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Office.Server;
using Microsoft.Office.Server.Search;
using Microsoft.Office.Server.Search.Query;
using Microsoft.Office.Server.Search.Administration;

public partial class _Default : System.Web.UI.Page
{
    protected void cmdSearch_Click(object sender, EventArgs e)
    {
        if (txtSearch.Text != "")
        {
            performSearch();
        }
        else
        {
            lblMsg.Text = "Please enter a search string";
        }
    }

    private void performSearch()
    {
        // Step 1: Get a handle to the Shared Services Search context
        ServerContext context = ServerContext.GetContext("SharedServices1");

        // Step 2: Construct a keyword search query
        KeywordQuery kwq = new KeywordQuery(context);
        kwq.ResultTypes = ResultType.RelevantResults;
        kwq.EnableStemming = true;
        kwq.TrimDuplicates = true;
        kwq.QueryText = txtSearch.Text;
        kwq.Timeout = 60000;
        kwq.RowLimit = 1000;
        kwq.KeywordInclusion = KeywordInclusion.AllKeywords;

        // Add the specific managed properties to output
        kwq.SelectProperties.Add("Title");
        kwq.SelectProperties.Add("Rank");
        kwq.SelectProperties.Add("Size");
        kwq.SelectProperties.Add("Write");
        kwq.SelectProperties.Add("Path");
        kwq.SelectProperties.Add("HitHighlightedSummary");
        kwq.SelectProperties.Add("ContentSource");

        // Step 3: Get the results to a DataTable
        ResultTableCollection results = kwq.Execute();
        ResultTable resultTable = results[ResultType.RelevantResults];
        DataTable dtResults = new DataTable();
        dtResults.Load(resultTable);

        // Step 4: Format summary
        foreach (DataRow drResult in dtResults.Rows)
        {
            drResult["HitHighlightedSummary"] = formatSummary(drResult["HitHighlightedSummary"].ToString());
        }

        // Step 6: Write out table of results
        gridSearch.DataSource = dtResults;
        gridSearch.DataBind();

        // Step 7: Inform the user how many hits were found
        lblMsg.Text = dtResults.Rows.Count.ToString() + " hits";
    }

    // Step 5: Highlight first 4 hits
    //  SharePoint Search places <c[#]> tags around the
    //  first 10 words in the summary that match
    //  a keyword search term.  Here I just find
    //  the first four and replace them with
    //  a <SPAN> element to show the hits in
    //  bold with a yellow background
    string formatSummary(string strSummary)
    {
        strSummary = strSummary.Replace("<c0>", "<span style='font-weight:bold; background-color:yellow'>");
        strSummary = strSummary.Replace("</c0>","</span>");

        strSummary = strSummary.Replace("<c1>", "<span style='font-weight:bold; background-color:yellow'>");
        strSummary = strSummary.Replace("</c1>", "</span>");
        
        strSummary = strSummary.Replace("<c2>", "<span style='font-weight:bold; background-color:yellow'>");
        strSummary = strSummary.Replace("</c2>", "</span>");
        
        strSummary = strSummary.Replace("<c3>", "<span style='font-weight:bold; background-color:yellow'>");
        strSummary = strSummary.Replace("</c3>", "</span>");
        
        return strSummary;
    }
}
