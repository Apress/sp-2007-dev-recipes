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
using System.Text;

public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {

        //Step 1: Open site collection and web site
        // for which we want to report on list storage
        SPSite site = new SPSite("http://localhost");
        SPWeb web = site.RootWeb;

        //Step 2: Get collection of all lists
        SPListCollection lists = web.Lists;

        //Step 3: iterate through all lists, finding
        // those which are document librarys
        DataTable dtListItems;
        foreach (SPList list in lists)
        {
            //Step 4: Is this a document library?
            if (list.BaseTemplate == SPListTemplateType.DocumentLibrary)
            {

                //Step 5: Get list of all documents in library
                dtListItems = list.Items.GetDataTable();

                //Step 6: is there at least one document in
                //  the library?
                if (dtListItems != null)
                {

                    //Step 7: Add heading
                    Label lbl = new Label();
                    lbl.Text = "<h1>" + list.Title + "</h1>";
                    this.Controls.Add(lbl);

                    //Step 8: Select just the desired columns
                    dtListItems = FormatTable(dtListItems);

                    //Step 9: Create XML representation of document list
                    StringBuilder sb = new StringBuilder();                    
                    System.IO.StringWriter sw = new System.IO.StringWriter(sb);
                    dtListItems.WriteXml(sw);

                    //Step 10: Format XML using XSLT
                    Xml xmlListItems = new Xml();
                    xmlListItems.DocumentContent = sb.ToString();
                    xmlListItems.TransformSource = "Recipes.xsl";
                    this.Controls.Add(xmlListItems);

                }

            }
        }
    }

    private DataTable FormatTable(DataTable dtListItems)
    {
        DataTable dtMyList = new DataTable("ListItems");
        dtMyList.Columns.Add("ID");
        dtMyList.Columns.Add("FileName");
        dtMyList.Columns.Add("FileSize");
        foreach (DataRow drListItem in dtListItems.Rows)
        {
            DataRow drMyListItem = dtMyList.NewRow();
            drMyListItem["ID"] = drListItem["ID"];
            drMyListItem["FileName"] = drListItem["LinkFileName"];
            drMyListItem["FileSize"] = drListItem["FileSizeDisplay"];
            dtMyList.Rows.Add(drMyListItem);
            dtMyList.AcceptChanges();
        }
        return dtMyList;
    }
}
