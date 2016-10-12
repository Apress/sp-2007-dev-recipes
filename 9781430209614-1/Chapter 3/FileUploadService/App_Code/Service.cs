using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class Service : System.Web.Services.WebService
{
    public Service () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string UploadFile2SharePoint(string fileName, byte[] fileContents, string siteUrl, string webName, string pathFolder)
    {
        // Step 1: Make sure a valid file has been passed to the service method
        if (fileContents == null)
        {
            return "Missing File";
        }
        try
        {
            // Step 2: Open the target site and web
            SPSite site = new SPSite(siteUrl);
            SPWeb web = site.AllWebs[webName];

            // Step 3: Open the folder to hold the document
            SPFolder folder = web.GetFolder(EnsureParentFolder(web,pathFolder+"/"+fileName));
            bool boolOverwrite = true;

            // Step 4: Add the file
            SPFile file = folder.Files.Add(fileName, fileContents, boolOverwrite);

            // Step 5: Declare victory!
            return "'" + file.Name + "' successfuly written to '" + file.Item.Url + "'";

        }
        catch (System.Exception ex)
        {
            return ex.Message;
        }
    }

    // This is a stock function from the WSS SDK to make
    // sure that a folder path exists before we try to upload the
    // file.
    public string EnsureParentFolder(SPWeb parentSite, string destinUrl)
    {
        destinUrl = parentSite.GetFile(destinUrl).Url;

        int index = destinUrl.LastIndexOf("/");
        string parentFolderUrl = string.Empty;

        if (index > -1)
        {
            parentFolderUrl = destinUrl.Substring(0, index);

            SPFolder parentFolder
                = parentSite.GetFolder(parentFolderUrl);

            if (!parentFolder.Exists)
            {
                SPFolder currentFolder = parentSite.RootFolder;

                foreach (string folder in parentFolderUrl.Split('/'))
                {
                    currentFolder
                        = currentFolder.SubFolders.Add(folder);
                }
            }
        }
        return parentFolderUrl;
    }

}
