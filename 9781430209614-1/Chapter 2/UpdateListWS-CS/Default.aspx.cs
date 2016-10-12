using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Xml;

public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            // Populate the "Command" drop-down list
            ddlCommand.Items.Add("Delete");
            ddlCommand.Items.Add("New");
            ddlCommand.Items.Add("Update");
            ddlCommand.SelectedIndex = 2;

            // Populate the "ID" drop-down list
            ddlID = GetIDs();

            // Get Current entries in list
            RefreshEmployeeList();
        }
    }

    protected void ddlCommand_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlID = GetIDs();
    }

    DataTable GetAllEmployees()
    {
        ListsService.Lists objListService = new ListsService.Lists();
        objListService.Url = "http://localhost/_vti_bin/lists.asmx";
        objListService.Credentials = System.Net.CredentialCache.DefaultCredentials;

        DataTable dtEmployees = new DataTable("Employees");
        dtEmployees.Columns.Add("ID");
        dtEmployees.Columns.Add("EmpName");
        dtEmployees.Columns.Add("JobTitle");
        dtEmployees.Columns.Add("HireDate");
        DataRow drEmployee;
        XmlNode xnEmployees = objListService.GetListItems("Employee", null, null, null, null, null, null);
        foreach (XmlNode xnEmployee in xnEmployees.ChildNodes[1].ChildNodes)
        {
            try
            {
                drEmployee = dtEmployees.NewRow();
                drEmployee["ID"] = xnEmployee.Attributes["ows_ID"].Value;
                drEmployee["EmpName"] = xnEmployee.Attributes["ows_EmpName"].Value;
                drEmployee["JobTitle"] = xnEmployee.Attributes["ows_JobTitle"].Value;
                drEmployee["HireDate"] = xnEmployee.Attributes["ows_HireDate"].Value;
                dtEmployees.Rows.Add(drEmployee);
            }
            catch { }
        }

        return dtEmployees;
            
    }

    // Return a drop-down list object containing
    // all current IDs, unless the "New" command
    // selected, in which case the only valud
    // value for ID is also "New"
    DropDownList GetIDs()
    {
        ddlID.Items.Clear();
        if (ddlCommand.SelectedValue == "New")
        {
            ListItem li = new ListItem("New", "New");
            ddlID.Items.Add(li);
        }
        else
        {
            DataTable dtEmployess = new DataTable();
            dtEmployess = GetAllEmployees();
            foreach (DataRow drEmployee in dtEmployess.Rows)
            {
                ListItem li = new ListItem(drEmployee["ID"].ToString(), drEmployee["ID"].ToString());
                ddlID.Items.Add(li);
            }
        }
        return ddlID;
    }
    
    // Redraw grid-view listing all employees
    void RefreshEmployeeList()
    {
        DataTable dtEmployeeListData = new DataTable();
        dtEmployeeListData = GetAllEmployees();
        this.GridView1.DataSource = dtEmployeeListData;
        this.GridView1.DataBind();
    }

    // Build necessary batch XML and call the web service method
    void UpdateListWS(string listName, DataTable dtListData)
    {

        // Step 1: create a reference to the "Lists" web service
        ListsService.Lists objListService = new ListsService.Lists();
        objListService.Url = "http://localhost/_vti_bin/lists.asmx";
        objListService.Credentials = System.Net.CredentialCache.DefaultCredentials;

        // Step 2: loop through rows in data table,
        //  adding one add, edit or delete command for each row
        DataRow drListItem;
        string strBatch = "";
        for (int i = 0; i < dtListData.Rows.Count; i++)
        {
            drListItem = dtListData.Rows[i];
            
            // Step 3: create a "Method" element to add to batch
            // Assume that first column of data table was the 'Cmd'
            strBatch += "<Method ID='" + i.ToString() + "' Cmd='" 
                + drListItem["Cmd"] + "'>";
            
            // Step 4: loop through fields 2-n, building
            //  one "method" in batch
            for (int j = 1; j < drListItem.Table.Columns.Count; j++)
            {
                // Only include columns with data
                if (drListItem[j].ToString() != "")
                {
                    strBatch += "<Field Name='" 
                        + drListItem.Table.Columns[j].ColumnName + "'>";
                    strBatch += Server.HtmlEncode(drListItem[j].ToString());
                    strBatch += "</Field>";
                }
            }

            // Step 5: close out this method entry
            strBatch += "</Method>";
        }

        // Step 6: Create the parent "batch" element
        XmlDocument xmlDoc = new System.Xml.XmlDocument();
        System.Xml.XmlElement xmlBatch = xmlDoc.CreateElement("Batch");

        // Step 7: tell SharePoint to keep processing if a single
        // "Method" causes an error.
        xmlBatch.SetAttribute("OnError", "Continue");
        xmlBatch.SetAttribute("ListVersion", "1");
        xmlBatch.SetAttribute("ViewName", "");

        // Step 8: add method (i.e. add/update/delete command) to batch
        xmlBatch.InnerXml = strBatch;

        // Step 9: process the batch
        XmlNode xmlReturn = objListService.UpdateListItems(listName, xmlBatch);

        // Display batch that was just run on web page
        lblBatchXML.Text = "<strong>Batch just processed</strong><br/><br/>" 
            + Server.HtmlEncode(xmlBatch.OuterXml);

        //Display the returned results
        lblReturnXML.Text = "<strong>Results</strong><br/><br/>" 
            + Server.HtmlEncode(xmlReturn.InnerXml);

    }

    protected void Button1_Click(object sender, EventArgs e)
    {

        // Define table to hold data to process
        DataTable dtEmployees = new DataTable("Employee");
        dtEmployees.Columns.Add("Cmd");          // New, Update, or Delete
        dtEmployees.Columns.Add("ID");           // New if adding, or ID of item
        dtEmployees.Columns.Add("Title");        // Builtin Title column
        dtEmployees.Columns.Add("EmpName");      // Employee name
        dtEmployees.Columns.Add("HireDate");     // Employee hire date
        dtEmployees.Columns.Add("JobTitle");     // Employee title

        // Call routine to update list
        DataRow drEmployee = dtEmployees.NewRow();
        drEmployee["Cmd"] = ddlCommand.SelectedValue;
        drEmployee["ID"] = ddlID.SelectedValue;
        drEmployee["EmpName"] = txtEmpName.Text;
        drEmployee["JobTitle"] = txtJobTitle.Text;
        drEmployee["HireDate"] = txtHireDate.Text;
        dtEmployees.Rows.Add(drEmployee);

        // Update SharePoint
        UpdateListWS("Employee", dtEmployees);

        RefreshEmployeeList();
    
    }
}

