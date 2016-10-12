using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;

namespace WebPropertiesWebPartCS
{
    public class WebPropertiesWebPart : WebPart
    {
        // Define location variables
        bool _debug = false;
        string _prefix = "_mg";

        [Personalizable]
        [WebBrowsable]
        [WebDescription("Check to display debug information")]
        [WebDisplayName("Debug?")]
        public bool Debug
        {
            get { return _debug; }
            set { _debug = value; }
        }

        [Personalizable]
        [WebBrowsable]
        [WebDescription("Prefix to use when storing and retrieving properties")]
        [WebDisplayName("Property prefix: ")]
        public string Prefix
        {
            get { return _prefix; }
            set { _prefix = value; }
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            try
            {
                // Step 1: Create table to hold existing, new properties
                Table tbl = new Table();
                TableRow tr;
                TableCell tc;
                ImageButton delBtn;
                TextBox tb;
                Label lbl;
                int i = 1;

                // Just a bit of formatting
                tbl.CellPadding = 3;
                tbl.CellSpacing = 3;
                tbl.BorderStyle = BorderStyle.Solid;

                // Add a heading
                tr = new TableRow();
                // #
                tc = new TableCell();
                tr.Cells.Add(tc);
                // Key
                tc = new TableCell();
                tc.Text = "Property Key";
                tc.Font.Bold = true;
                tr.Cells.Add(tc);
                // Value
                tc = new TableCell();
                tc.Text = "Value";
                tc.Font.Bold = true;
                tr.Cells.Add(tc);
                tbl.Rows.Add(tr);
                // Delete button
                tc = new TableCell();
                tr.Cells.Add(tc);

                tc.Font.Bold = true;
                tr.Cells.Add(tc);
                tbl.Rows.Add(tr);

                // Step 2: Loop through existing properties that match prefix
                // and are not null, add to table
                SPWeb web = SPControl.GetContextWeb(Context);
                SPPropertyBag properties = web.Properties;
                bool isAdmin = web.CurrentUser.IsSiteAdmin;

                foreach (object key in properties.Keys)
                {
                    if (key.ToString().IndexOf(_prefix) == 0 && properties[key.ToString()] != null)
                    {
                        // Create a new row for current property
                        tr = new TableRow();

                        // #
                        tc = new TableCell();
                        tc.Text = i.ToString() + ". ";
                        tr.Cells.Add(tc);

                        // Key
                        tc = new TableCell();
                        tc.Text = key.ToString().Substring(_prefix.Length);
                        tc.ID = "key_" + i.ToString();
                        tr.Cells.Add(tc);

                        // Value
                        tc = new TableCell();

                        // 3. For admin users, show value in
                        // an editable text box + delete button
                        if (isAdmin) 
                        {
                            tb = new TextBox();
                            tb.Text = properties[key.ToString()];
                            tb.ID = "value_" + i.ToString();
                            tc.Controls.Add(tb);
                            tr.Cells.Add(tc);

                            tc = new TableCell();
                            delBtn = new ImageButton();
                            delBtn.ImageUrl = "/_layouts/images/delete.gif";
                            delBtn.Click += new System.Web.UI.ImageClickEventHandler(delBtn_Click);
                            delBtn.ID = "delete_" + i.ToString();
                            tc.Controls.Add(delBtn);
                            tr.Cells.Add(tc);
                        }
                        else // for non-admin users, just show read-only
                        {
                            lbl = new Label();
                            lbl.Text = properties[key.ToString()];
                            tc.Controls.Add(lbl);
                            tr.Cells.Add(tc);
                        }

                        // Add new row to table
                        tbl.Rows.Add(tr);
                        i++;
                    }
                }

                // Step 4: Add a final row to allow user 
                // to add new properties if current user is site admin
                if (isAdmin)
                {
                    tr = new TableRow();

                    // #
                    tc = new TableCell();
                    tc.Text = "*. ";
                    tr.Cells.Add(tc);

                    // Key
                    tc = new TableCell();
                    tb = new TextBox();
                    tb.Text = "";
                    tb.ID = "key_new";
                    tc.Controls.Add(tb);
                    tr.Cells.Add(tc);

                    // Value
                    tc = new TableCell();
                    tb = new TextBox();
                    tb.Text = "";
                    tb.ID = "value_new";
                    tc.Controls.Add(tb);
                    tr.Cells.Add(tc);
                    tbl.Rows.Add(tr);

                }

                // Step 5: Add the completed table to the page
                this.Controls.Add(tbl);

                // Step 6: Now add a button to save changes, 
                // if current user is site admin
                if (isAdmin)
                {
                    lbl = new Label();
                    lbl.Text = "<br/>";
                    this.Controls.Add(lbl);
                    Button btn = new Button();
                    btn.Text = "Save changes";
                    btn.Click += new EventHandler(btn_Click);
                    this.Controls.Add(btn);
                }

            }
            catch (Exception ex)
            {
                Label lbl = new Label();
                lbl.Text = "Error: " + ex.Message;
                this.Controls.Add(lbl);
            }

        }

        // Handles "Save Changes" button click event
        void btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Step 1: Get handle to web site property
                // collection
                bool isChanged = false;
                SPWeb web = SPControl.GetContextWeb(Context);
                SPPropertyBag properties = web.Properties;
                web.AllowUnsafeUpdates = true;

                // Step 2: Add new property
                TextBox tbNewKey = (TextBox)this.FindControl("key_new");
                TextBox tbNewValue = (TextBox)this.FindControl("value_new");
                if (tbNewKey.Text != "")
                {
                    properties[_prefix+tbNewKey.Text] = tbNewValue.Text;
                    web.Properties.Update();
                    isChanged = true;
                }

                // Step 3: Loop through textboxes in web part
                // updating corresponding site property if
                // checkbox has been changed.
                TableCell tc;
                TextBox tb;
                for (int i = 1; i < 999; i++)
                {
                    tc = (TableCell)this.FindControl("key_"+i.ToString());

                    // Step 4: If a control with the name "key_<n>" exists, get it,
                    // otherwise assume no more custom properties to edit
                    if (tc != null)
                    {
                        // Step 5: Ok, we found the textbox containing the property
                        // value, now let's see if the value in the textbox
                        // has been changed to something other than that in
                        // the corresponding web property.
                        tb = (TextBox)this.FindControl("value_" + i.ToString());
                        if (properties[_prefix+tc.Text].Trim() != tb.Text.Trim())
                        {
                            // Step 6: The value was changed, update the web property
                            // and set the flag indicating that web part
                            // needs to be redrawn
                            properties[_prefix+tc.Text] = tb.Text;
                            web.Properties.Update();
                            isChanged = true;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                // Step 7: If any changes made, redraw web part
                // to reflect changed/added properties
                if (isChanged)
                {
                    this.Controls.Clear();
                    CreateChildControls();
                }
            }
            catch (Exception ex)
            {
                Label lbl = new Label();
                lbl.Text = "<br/><br/>Error: " + ex.Message;
                this.Controls.Add(lbl);
            }
        }

        // Handles individual property delete button click
        void delBtn_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            try
            {
                // Step 1. Get handle to name of property to be
                // deleted
                ImageButton delBtn = (ImageButton)sender;
                string _id = delBtn.ID.Replace("delete_", "");
                TableCell tc = (TableCell)this.FindControl("key_" + _id);

                // Step 2: Get handle to web site, property collection
                SPWeb web = SPControl.GetContextWeb(Context);
                SPPropertyBag properties = web.Properties;
                web.AllowUnsafeUpdates = true;

                // Step 3: Delete the unwanted property by setting 
                // it's value to null (note: for some reason using
                // the Remove() method was not sufficient to cause
                // SharePoint to delete the property.
                web.Properties[_prefix + tc.Text] = null;
                web.Properties.Update();
                web.Update();

                // Step 4: Refresh list
                this.Controls.Clear();
                CreateChildControls();

                // Step 5: Display message to user informing them
                // that property has been deleted
                Label lbl = new Label();
                lbl.Text = "<br/><br/>You deleted property '" + tc.Text + "'";
                this.Controls.Add(lbl);
            }
            catch (Exception ex)
            {
                Label lbl = new Label();
                lbl.Text = "<br/><br/>Error: " + ex.Message;
                this.Controls.Add(lbl);
            }
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            base.RenderContents(writer);

            if (_debug)
            {
                writer.Write("<hr/>");
                writer.Write("<strong>Prefix:</strong> " + _prefix);
                writer.Write("<hr/>");
            }
        }

    }
}
