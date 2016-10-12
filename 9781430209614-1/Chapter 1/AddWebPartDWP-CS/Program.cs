using System.Xml;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using System.IO;
using System;

class Module1
{

    static void Main(string[] args)
    {
        if ((args.Length < 6))
        {
            GetArgs(ref args);
        }
        Console.WriteLine(
            AddWebPartDWP(
                args[0], 
                args[1], 
                args[2], 
                args[3], 
                PersonalizationScope.Shared, 
                args[4], 
                args[5]));

        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.Read();
    }

    private static void GetArgs(ref string[] args)
    {
        args = new string[6];
        Console.WriteLine();
        Console.Write("Site Url: ");
        args[0] = Console.ReadLine();
        Console.Write("Web Name: ");
        args[1] = Console.ReadLine();
        Console.Write("Doclib containing page (leave blank for none): ");
        args[2] = Console.ReadLine();
        Console.Write("Page Name: ");
        args[3] = Console.ReadLine();
        Console.Write("Path to DWP: ");
        args[4] = Console.ReadLine();
        Console.Write("Zone: ");
        args[5] = Console.ReadLine();
        Console.WriteLine();
    }

    public static string AddWebPartDWP( 
        string strSiteUrl, 
        string strWebName, 
        string strDocLibName, 
        string strPage, 
        System.Web.UI.WebControls.WebParts.PersonalizationScope pScope, 
        string strDWPPath, 
        string strZone)
    {
        try
        {
            // Get handle to site and web to be edited
            SPSite site = new SPSite(strSiteUrl);
            SPWeb web = site.OpenWeb(strWebName);
            System.Web.UI.WebControls.WebParts.WebPart wp;

            // Step 1: Get handle to web part page to contain new web part
            SPLimitedWebPartManager webParts;
            if ((strDocLibName != ""))
            {
                webParts = web.GetLimitedWebPartManager(strDocLibName + "/" + strPage, pScope);
            }
            else
            {
                webParts = web.GetLimitedWebPartManager(strPage, pScope);
            }

            // Step 2: get handle to .DWP file and import definition
            XmlReader reader = XmlReader.Create(strDWPPath);
            string strErrMsg;
            wp = webParts.ImportWebPart(reader, out strErrMsg);

            // Step 3: allow web site to be edited
            web.AllowUnsafeUpdates = true;
            SPList list = null;
            bool origForceCheckoutValue = false;
            if (strDocLibName != "")
            {
                list = web.Lists[strDocLibName];
                origForceCheckoutValue = list.ForceCheckout;
                list.ForceCheckout = false;
                list.Update();
            }

            // Step 6: Add the web part
            webParts.AddWebPart(wp, strZone, 0);

            // Step 7: Save changes back to SharePoint database
            webParts.SaveChanges(wp);
            web.Update();

            // Step 8-9: If necessary, restore ForceCheckout setting
            if (strDocLibName != "")
            {
                list.ForceCheckout = origForceCheckoutValue;
                list.Update();
            }
            return ("Successfuly added \'" + strDWPPath + "\'");
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}