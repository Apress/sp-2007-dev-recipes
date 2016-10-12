using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace ConnectablePageViewerCS
{
    // Step 1: Create the Interface
    // ----------------------------
    // The interface is the glue between the "provider" web part, 
    // that sends data to the "consumer" web page.  It provides
    // a structure in which to pass the data.  In this case
    // we're just passing a single string value that represents
    // the Url to display, but we could pass multiple data
    // items by providing multiple properties
    public interface IUrl
    {
        string Url { get; }
    }

    // Step 2: Create the provider web part class
    // ------------------------------------------
    // The "provider" web part will display a drop-down list
    // or site-name/url pairs.  When the user selects a value
    // the selected Url will be passed to the "consumer".
    public class UrlProvider : WebPart, IUrl
    {
        DropDownList ddlUrl = null;
        string _urls =
            "Microsoft;http://www.microsoft.com;Yahoo!;http://www.yahoo.com";

        // The "Urls" property will store a semi-conlon delimeted
        // list of site-name/url pairs to populate the drop-down list
        [Personalizable]
        [WebBrowsable]
        public string Urls
        {
            get { return _urls; }
            set { _urls = value; }
        }

        // Step 3: Override the “CreateChildControls()” method
        // ---------------------------------------------------
        // The CreateChildControls() base method is called
        // to populate the drop-down list of sites and
        // add to the web part output
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            try
            {
                // Create the drop-down list of urls from
                // the parsed string in "Urls" property
                string[] arrUrls = _urls.Split(';');
                ListItem li;
                ddlUrl = new DropDownList();
                ddlUrl.Items.Add(new ListItem("[Please select a Url]", ""));
                for (int i = 0; i < arrUrls.Length; i = i + 2)
                {
                    li = new ListItem(arrUrls[i], arrUrls[i + 1]);
                    ddlUrl.Items.Add(li);
                }
                ddlUrl.Items[0].Selected = true;
                ddlUrl.AutoPostBack = true;

                this.Controls.Add(ddlUrl);
            }
            catch (Exception ex)
            {
                Label lbl = new Label();
                lbl.Text = ex.Message;
                this.Controls.Add(lbl);
            }
        }

        // Step 4: Define any methods required by the interface
        // ----------------------------------------------------
        // This is the single method that was
        // specified in the Interface, and must be provided
        // to pass the selected url to the "consumer" web
        // part
        public string Url
        {
            get { return ddlUrl.SelectedValue; }
        }

        // Step 5: Define and “decorate” the ConnectionProvider method
        // -----------------------------------------------------------
        // This method is required to wire-up the
        // "provider" with one or more "consumers".
        // Note the "ConnectionProvider" decoration,
        // that tells .NET to make this the provider's
        // connection point
        [ConnectionProvider("Url Provider")]
        public IUrl GetUrl()
        {
            return this;
        }
    }

    // Step 6: Define the consumer web part class
    // ------------------------------------------
    // This class defines the "consumer" web part that will
    // obtain the url from the "provider"
    public class ConnectablePageViewer : WebPart
    {
        string _url = "";

        // Step 7: Override either or both the CreateChildControls() and/or
        //		 RenderContents() base methods
        // ----------------------------------------------------------------
        // In the RenderContents() method we get the url value
        // which has been written to the _url local variable by
        // the "UrlConsumer()" method that automatically fires
        // when this web part is wired-up with a "provider"
        protected override void RenderContents(
            System.Web.UI.HtmlTextWriter writer)
        {
            base.RenderContents(writer);
            try
            {
                if (_url != "")
                {
                    // Create an <IFRAME> HTML tag and set the 
                    // source to the selected url
                    writer.Write("Opening page: " + _url);
                    writer.Write("<hr/>");
                    writer.Write("<div>");
                    writer.Write("<iframe src='" + _url +
                        "' width='100%' height='100%'></iframe>");
                    writer.Write("</div>");
                }
                else
                {
                    writer.Write("Please select a Url from the provider.");
                }
            }
            catch (Exception ex)
            {
                writer.Write(ex.Message);
            }
        }

        // Step 8: Define a ConnectionConsumer() method to receive
        // 		 data from the provider
        // -------------------------------------------------------
        // The UrlConsumer() method is wired-up using the
        // "ConnectionConsumer()" decoration, that tells
        // .NET to automatically fire this method when
        // the consumer is connected to a provider
        [ConnectionConsumer("Url Consumer")]
        public void UrlConsumer(IUrl url)
        {
            try
            {
                _url = url.Url;
            }
            catch (Exception ex)
            {
                // No op
            }
        }
    }
}
