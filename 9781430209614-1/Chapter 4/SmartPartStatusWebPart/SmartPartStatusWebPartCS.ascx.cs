using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

partial class SmartPartStatusWebPartCS : System.Web.UI.UserControl
{

    //Define local variables 
    private bool _showImage = true;
    private string _statusListSiteUrl = "http://localhost";
    private string _statusListWeb = "spwp";
    private string _statusList = "Status";

    private void Page_PreRender(object sender, System.EventArgs e)
    {

        //Only run this code if this is the first time the 
        //user control has been displayed on the current 
        //page since it was opened 
        if (!IsPostBack)
        {
            HideAllImages();
            switch (GetProfileStatus())
            {
                case "Green":
                    this.imgGreen.Visible = true;
                    this.radGreen.Checked = true;
                    break;
                case "Yellow":
                    this.imgYellow.Visible = true;
                    this.radYellow.Checked = true;
                    break;
                case "Red":
                    this.imgRed.Visible = true;
                    this.radRed.Checked = true;
                    break;
                default:
                    break;
            }
        }

    }

    private string GetProfileStatus()
    {
        try
        {
            SPSite site = new SPSite(_statusListSiteUrl);
            SPWeb web = site.AllWebs[_statusListWeb];
            SPList list = web.Lists[_statusList];

            //Find the list item for the current user, and update its status 
            //web.AllowUnsafeUpdates = True 
            foreach (SPListItem ListItem in list.Items)
            {
                try
                {
                    if (ListItem["UserAlias"].ToString().ToLower() == Context.User.Identity.Name.ToLower())
                    {
                        return ListItem["Status"].ToString();
                        break; 
                    }
                }
                catch (Exception ex)
                {
                    //No op 
                }
            }
        }
        catch (Exception ex)
        {
            //No op 
        }

        //If we got this far, no entry was found for the current user 
        return "";

    }

    private void UpdateStatus() 
    { 
        try { 
            //Get a handle to the list that we're using to store 
            //user status information 
            SPSite site = new SPSite(_statusListSiteUrl); 
            SPWeb web = site.AllWebs[_statusListWeb]; 
            SPList list = web.Lists[_statusList];
            SPListItem listItem;
            bool boolFound = false; 
            
            //Find the list item for the current user, and update its status 
            foreach (SPListItem li in list.Items) { 
                try { 
                    if (li["UserAlias"].ToString().ToLower() == Context.User.Identity.Name.ToLower()) { 
                        li["Status"] = GetUserControlStatus(); 
                        li.Update(); 
                        boolFound = true; 
                        break; 
                    } 
                } 
                catch (Exception ex) { 
                } 
            } 
            
            //If an entry for the current user wasn't found in the list 
            //add one now. 
            if (!boolFound) {
                listItem = list.Items.Add(); 
                listItem["UserAlias"] = Context.User.Identity.Name; 
                listItem["Status"] = GetUserControlStatus(); 
                listItem.Update(); 
            } 
        } 
        
        catch (Exception ex) { 
            
            Label lbl = new Label(); 
            lbl.Text = "<font color='red'>" + ex.Message + "</font><br/>"; 
            this.Controls.Add(lbl); 
            
        } 
        
    }

    //Get the currently selected status from 
    //the user control UI 
    private string GetUserControlStatus()
    {
        if (radRed.Checked)
        {
            return "Red";
        }
        else if (radYellow.Checked)
        {
            return "Yellow";
        }
        else
        {
            return "Green";
        }
    }

    //Helper function to make sure all images are 
    //hidden prior to displaying the selected one 
    public void HideAllImages()
    {
        this.imgGreen.Visible = false;
        this.imgYellow.Visible = false;
        this.imgRed.Visible = false;
    }

    //The following event handlers process button clicks to 
    //display the image corresponding to the selected status 
    public void radGreen_CheckedChanged(object sender, EventArgs e)
    {
        HideAllImages();
        if (radGreen.Checked)
        {
            if (_showImage)
            {
                imgGreen.Visible = true;
            }
        }
        UpdateStatus();
    }
    public void radYellow_CheckedChanged(object sender, EventArgs e)
    {
        HideAllImages();
        if (radYellow.Checked)
        {
            if (_showImage)
            {
                imgYellow.Visible = true;
            }
        }
        UpdateStatus();
    }
    public void radRed_CheckedChanged(object sender, EventArgs e)
    {
        HideAllImages();
        if (radRed.Checked)
        {
            if (_showImage)
            {
                imgRed.Visible = true;
            }
        }
        UpdateStatus();
    }

}