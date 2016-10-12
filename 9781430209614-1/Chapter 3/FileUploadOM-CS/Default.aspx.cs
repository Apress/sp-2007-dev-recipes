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
using System.IO;

public partial class _Default : System.Web.UI.Page 
{
    protected void cmdUploadFile_Click(object sender, EventArgs e)
    {
        try
        {
            lblErrorMsg.Visible = false;

            // Step 1: Get handle to site collection, web site, list 
            SPSite site = new SPSite(txtSiteUrl.Text);
            SPWeb web = site.AllWebs[txtWebName.Text];
            SPList dl = web.Lists[txtDocLibName.Text];
            SPFile file;

            web.AllowUnsafeUpdates = true;
            web.Lists.IncludeRootFolder = true;

            // Step 2: Make sure user has selected a file 
            if (FileUpload1.PostedFile.FileName != "")
            {

                // Step 3: load the content of the file into a byte array
                Stream fStream;
                Byte[] contents = new Byte[FileUpload1.PostedFile.InputStream.Length];

                fStream = FileUpload1.PostedFile.InputStream;
                fStream.Read(contents, 0, (int)fStream.Length);
                fStream.Close();

                // Step 4: upload the file to SharePoint doclib
                file = web.Files.Add(web.Url.ToString() + "/" + dl.Title.ToString() + "/" + FileUpload1.FileName, contents);
                file.Update();

            }
            else
            {
                lblErrorMsg.Text = "Please select a file to upload";
                lblErrorMsg.Visible = true;
            }
        }
        catch (Exception ex)
        {
            lblErrorMsg.Text = ex.Message;
            lblErrorMsg.Visible = true;
        }
                
    }
}
