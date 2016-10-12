using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class Service : System.Web.Services.WebService
{

    private string _retVal = "";
    private string _siteUrl = "";
    private string _webName = "";
    private string _title = "";
    private string _description = "";
    private string _webTemplate;

    [WebMethod]
    public string CreateWeb(string strSiteUrl, string strWebName, string strTitle, string strDescription, string strWebTemplate)
    {
        _siteUrl = strSiteUrl;
        _webName = strWebName;
        _title = strTitle;
        _description = strDescription;
        _webTemplate = strWebTemplate;
        // Step 1: Verify that all arguments have been passed
        if (_siteUrl != ""
            && _webName != ""
            && _title != ""
            && _description != ""
            && _webTemplate != "")
        {
            // Run with permissions of collection administrator
            SPSecurity.RunWithElevatedPrivileges(myCreateWeb);
            return _retVal;
        }
        else
        {
            // 2. If some arguments are missing, return error message
            return "Missing arguments";
        }
    }

    private void myCreateWeb()
    {
        // Step 4: Trap error, if any
        try
        {
            SPSite site = new SPSite(_siteUrl);
            // Step 3: Add the new site to collection 
            site.AllWebs.Add(_webName, _title, _description, ((System.UInt32)(1033)), _webTemplate, false, false);
            // Step 5: Return success message
            _retVal = "Successfuly added new site";
        }
        catch (System.Exception ex)
        {
            // Step 6: Return error message
            _retVal = ex.Message;
        }
    }
}

