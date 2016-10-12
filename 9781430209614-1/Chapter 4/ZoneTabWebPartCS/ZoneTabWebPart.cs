using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Drawing;

namespace ZoneTabWebPartCS
{
    public class ZoneTabWebPart : WebPart
    {
        // Local variables
        string _tabData = "";
        bool _debug = false;
        string _selectedTab;
        int _tabWidth = 100;
        string _tabBackgroundColorSelected = "white";
        string _tabBackgroundColorDeselected = "whitesmoke";

        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("Flag indicating whether debug info should be displayed")]
        public bool Debug
        {
            get { return _debug; }
            set { _debug = value; }
        }

        // String containing semi-colon delimited list
        // of tab names.  Tab names are preceeded by "*".
        //
        // Example: *Tab 1;webpart1;webpart2;*Tab 2;webpart3
        //
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("A delimited list of tab names and associated web parts")]
        public string TabData
        {
            get { return _tabData; }
            set { _tabData = value; }
        }

        [Personalizable]
        [WebBrowsable]
        [WebDescription("Color of selected tab")]
        public string SelectedColor
        {
            get { return _tabBackgroundColorSelected; }
            set { _tabBackgroundColorSelected = value; }
        }

        [Personalizable]
        [WebBrowsable]
        [WebDescription("Color of un-selected tab")]
        public string DeSelectedColor
        {
            get { return _tabBackgroundColorDeselected; }
            set { _tabBackgroundColorDeselected = value; }
        }

        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("Width in pixels for each tab")]
        public int TabWidth
        {
            get { return _tabWidth; }
            set { _tabWidth = value; }
        }

        // Add tab-links to page
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            try
            {

                string[] arrTabs = _tabData.Split(';');

                // Build list of tabs in the form
                // of an HTML <TABLE> with <A> tags
                // for each tab

                // Step 1: Define <TABLE> and <TR> HTML elements
                Table tbl = new Table();
                tbl.CellPadding = 0;
                tbl.CellSpacing = 0;

                TableRow tr = new TableRow();
                tr.HorizontalAlign = HorizontalAlign.Left;

                // Step 2: Loop through list of tabs, adding
                //  <TD> and <A> HTML elements for each
                TableCell tc;
                LinkButton tab;
                int tabCount = 0;
                for (int i = 0; i < arrTabs.Length; i++)
                {
                    if (arrTabs[i].IndexOf("*") == 0)
                    {
                        // Add a blank separator cell
                        tc = new TableCell();
                        tc.Text = "&nbsp;";
                        tc.Width = System.Web.UI.WebControls.Unit.Percentage(1);
                        tc.Style["border-bottom"] = "black 1px solid";
                        tr.Cells.Add(tc);

                        // Step 4: Create a <TD> HTML element to hold the tab
                        tc = new TableCell();
                        tc.ID = "tc_" + arrTabs[i].Substring(1).Replace(" ", "_");
                        tc.Width = System.Web.UI.WebControls.Unit.Pixel(_tabWidth);

                        // Step 5: Create an <A> HTML element to represent
                        // the tab.  Discard first character, which
                        // was a "*"
                        tab = new LinkButton();
                        tab.ID = "tab_" + arrTabs[i].Substring(1).Replace(" ", "_");
                        tab.Text = arrTabs[i].Substring(1);

                        // Step 6: Attach event handler that will execute when
                        // user clicks on tab link
                        tab.Click += new EventHandler(tab_Click);

                        // Step 7: Set any other properties as desired
                        tab.Width = System.Web.UI.WebControls.Unit.Pixel(_tabWidth-2);
                        tab.Style["text-align"] = "center";
                        tab.Style["font-size"] = "larger";
                        
                        // Step 8: Insert tab <A> element into <TD> element
                        tc.Controls.Add(tab);

                        // Step 9: Insert <TD> element into <TR> element
                        tr.Cells.Add(tc);
                        tabCount++;
                    }
                }

                // Add final blank cell to cause horizontal line to
                // run across entire zone width
                tc = new TableCell();
                tc.Text = "&nbsp;";
                tc.Width = System.Web.UI.WebControls.Unit.Pixel(_tabWidth * 10);
                tc.Style["border-bottom"] = "black 1px solid";
                tr.Cells.Add(tc);

                // Step 10: Insert the <TR> element into <TABLE> and
                // add the HTML table to the page
                tbl.Rows.Add(tr);
                this.Controls.Add(tbl);

            }
            catch (Exception ex)
            {
                Label lbl = new Label();
                lbl.Text = "Error: " + ex.Message;
                this.Controls.Add(lbl);
            }
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            if (_debug)
            {
                writer.Write("Tab Data: " + _tabData + "<hr/>");
            }
            ShowHideWebParts(writer);
            base.RenderContents(writer);
        }

        // Show web parts for currently selected tab, 
        // hide all others
        void ShowHideWebParts(System.Web.UI.HtmlTextWriter writer)
        {
            try
            {
                Label lbl = new Label();
                string[] arrTabs = _tabData.Split(';');

                // Step 1: If a tab has not been selected, assume
                // the first one
                if (_selectedTab == null) _selectedTab = arrTabs[0].Substring(1);

                // Step 2: Hide all web parts in zone that are
                // below the ZoneTab part
                foreach (WebPart wp in this.Zone.WebParts)
                {
                    if (wp.ZoneIndex > this.ZoneIndex)
                    {
                        wp.Hidden = true;
                    }
                }

                // Step 3: Get web part names associated with this tab
                for (int i = 0; i < arrTabs.Length; i++)
                {
                    // Step 4: Find the selected tab
                    if (arrTabs[i] == "*" + _selectedTab)
                    {
                        // Step 5: Get associated web part names
                        for (int j = i + 1; j < arrTabs.Length; j++)
                        {
                            // Step 6: Loop until next tab name found, or end of list
                            if (arrTabs[j].IndexOf("*") != 0)
                            {
                                // Step 7: Show named web parts
                                foreach (WebPart wp in this.Zone.WebParts)
                                {
                                    if (wp.Title == arrTabs[j]) wp.Hidden = false;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        // Step 8: Bring tab border to "front"
                        TableCell tc = (TableCell)this.FindControl("tc_" + arrTabs[i].Substring(1).Replace(" ", "_"));
                        tc.Style["border-bottom"] = "white 1px solid";
                        tc.Style["border-top"] = "black 1px solid";
                        tc.Style["border-left"] = "black 1px solid";
                        tc.Style["border-right"] = "black 1px solid";
                        tc.Style["background-color"] = _tabBackgroundColorSelected;

                        // Step 9: Highlight selected tab
                        LinkButton tab = (LinkButton)this.FindControl("tab_" + arrTabs[i].Substring(1).Replace(" ", "_"));
                        tab.Style["background-color"] = _tabBackgroundColorSelected;
                    }
                    else
                    {
                        if (arrTabs[i].IndexOf("*") == 0)
                        {
                            // Step 10: Send tab border to "back"
                            TableCell tc = (TableCell)this.FindControl("tc_" + arrTabs[i].Substring(1).Replace(" ", "_"));
                            tc.Style["border-bottom"] = "black 1px solid";
                            tc.Style["border-top"] = "gray 1px solid";
                            tc.Style["border-left"] = "gray 1px solid";
                            tc.Style["border-right"] = "gray 1px solid";
                            tc.Style["background-color"] = _tabBackgroundColorDeselected;
                            
                            // Step 11: Lowlight selected tab
                            LinkButton tab = (LinkButton)this.FindControl("tab_" + arrTabs[i].Substring(1).Replace(" ", "_"));
                            tab.Style["background-color"] = _tabBackgroundColorDeselected;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                writer.Write("Error: " + ex.Message);
            }
        }

        // This is the click event handler that was assigned
        // to all tab LinkButton objects in CreateChildControls()
        // method.
        void tab_Click(object sender, EventArgs e)
        {
            try
            {
                // Set flag indicated current tab, for
                // use in RenderContents() method
                LinkButton tab = (LinkButton)sender;
                _selectedTab = tab.Text;
            }
            catch (Exception ex)
            {
                Label lbl = new Label();
                lbl.Text = ex.Message;
                this.Controls.Add(lbl);
            }
        }

    }
}
