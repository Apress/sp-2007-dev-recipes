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

            // Get list of current ID's
            GetIDs();

            // Populate form fields
            setFields();

            // Get Current entries in list
            RefreshEmployeeList();
        }
    }

    // Return list of employees currently in
    // the SharePoint list
    DataTable GetAllEmployees()
    {
        SPSite site = new SPSite("http://localhost");
        SPWeb web = site.AllWebs[""];
        SPList employees = web.Lists["Employee"];

        DataTable dtEmployees = employees.Items.GetDataTable();
        DataTable dtEmployeesNew = new DataTable("Employees");

        dtEmployeesNew.Columns.Add("ID");
        dtEmployeesNew.Columns.Add("EmpName");
        dtEmployeesNew.Columns.Add("JobTitle");
        dtEmployeesNew.Columns.Add("HireDate");
        
        foreach (DataRow drEmployee in dtEmployees.Rows)
        {
            try
            {
                DataRow drEmployeeNew = dtEmployeesNew.NewRow();
                drEmployeeNew["ID"] = drEmployee["ID"].ToString();
                drEmployeeNew["EmpName"] = drEmployee["EmpName"].ToString();
                drEmployeeNew["JobTitle"] = drEmployee["JobTitle"].ToString();
                drEmployeeNew["HireDate"] = drEmployee["HireDate"].ToString();
                dtEmployeesNew.Rows.Add(drEmployeeNew);
                dtEmployeesNew.AcceptChanges();
            }
            catch { }
        }

        return dtEmployeesNew;

    }

    // Return a drop-down list object containing
    // all current IDs, unless the "New" command
    // selected, in which case no ID is needed
    DropDownList GetIDs()
    {
        ddlID.Items.Clear();
        if (ddlCommand.SelectedValue == "New")
        {
            ddlID.Enabled = false;
            ddlID.Items.Add(new ListItem("N/A"));
        }
        else
        {
            ddlID.Enabled = true;
            DataTable dtEmployess = new DataTable();
            dtEmployess = GetAllEmployees();
            foreach (DataRow drEmployee in dtEmployess.Rows)
            {
                ListItem li = new ListItem(drEmployee["ID"].ToString(), drEmployee["ID"].ToString());
                ddlID.Items.Add(li);
            }
        }
        ddlID.SelectedIndex = 0;
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

    // Update form fields to reflect 
    // selected command and, if appropriate
    // selected ID
    private void setFields()
    {
        // Clear out data entry fields
        txtEmpName.Text = "";
        txtHireDate.Text = "";
        txtJobTitle.Text = "";
        lblReturnMsg.Text = "";

        // By default, let user select an existing ID
        ddlID.Enabled = true;

        // Enable or disable fields as appropriate
        if (ddlCommand.SelectedValue == "Delete")
        {
            txtEmpName.Enabled = false;
            txtHireDate.Enabled = false;
            txtJobTitle.Enabled = false;
        }
        else
        {
            // If "New", doesn't make sense for
            // user to select an ID
            if (ddlCommand.SelectedValue == "New")
                ddlID.Enabled = false;
            else
            {
                ddlID.Enabled = true;

                // Retrieve existing data for selected employee
                SPSite site = new SPSite("http://localhost");
                SPWeb web = site.AllWebs[""];
                SPList list = web.Lists["Employee"];
                int ID = int.Parse(ddlID.SelectedValue);
                SPListItem item = list.GetItemById(ID);

                // Assign form field values from SharePoint list
                txtEmpName.Text = item["EmpName"].ToString();
                txtHireDate.Text = item["HireDate"].ToString();
                txtJobTitle.Text = item["JobTitle"].ToString();
            }

            txtEmpName.Enabled = true;
            txtHireDate.Enabled = true;
            txtJobTitle.Enabled = true;
        }
    }

    protected void ddlCommand_SelectedIndexChanged(object sender, EventArgs e)
    {
        GetIDs();
        setFields();
    }

    protected void ddlID_SelectedIndexChanged(object sender, EventArgs e)
    {
        setFields();
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        try
        {
            SPSite site = new SPSite("http://localhost");
            SPWeb web = site.AllWebs[""];
            SPList list = web.Lists["Employee"];
            SPListItem item;
            int ID;
            lblReturnMsg.Text = "";

            web.AllowUnsafeUpdates = true;

            switch (ddlCommand.SelectedValue)
            {
                case "New":
                    item = list.Items.Add();
                    item["EmpName"] = txtEmpName.Text;
                    item["JobTitle"] = txtJobTitle.Text;
                    item["HireDate"] = txtHireDate.Text;
                    item.Update();
                    lblReturnMsg.Text = "'" + txtEmpName.Text + "' has been successfuly added";
                    break;
                case "Update":
                    ID = int.Parse(ddlID.SelectedValue);
                    item = list.GetItemById(ID);
                    item["EmpName"] = txtEmpName.Text;
                    item["JobTitle"] = txtJobTitle.Text;
                    item["HireDate"] = txtHireDate.Text;
                    item.Update();
                    lblReturnMsg.Text = "'" + txtEmpName.Text + "' has been successfuly updated";
                    break;
                case "Delete":
                    ID = int.Parse(ddlID.SelectedValue);
                    item = list.GetItemById(ID);
                    string empName = item["EmpName"].ToString();
                    list.Items.DeleteItemById(ID);
                    lblReturnMsg.Text = "'" + empName + "' has been successfuly deleted";
                    break;
            }
            list.Update();

            GetIDs();
            setFields();
            RefreshEmployeeList();
        }
        catch (Exception ex)
        {
            lblReturnMsg.Text = ex.Message;
        }
    }

}
