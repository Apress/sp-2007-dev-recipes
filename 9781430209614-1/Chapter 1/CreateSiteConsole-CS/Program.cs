using Microsoft.SharePoint;
using System;

//-------------------------------//
// Project: CreateSiteConsole-CS //
//-------------------------------//
class Module1
{

    static void Main(string[] args)
    {
        // Steps 1-2: If no arguments have been passed, prompt user to enter them now
        if ((args.Length == 0))
        {
            args = new string[7];
            args = GetParams(ref args);
        }
        // Add the new site (web) using the provided arguments
        AddSite(args);
    }

    private static void AddSite(string[] args)
    {

        try
        {
            // Get a handle to the site collection to which the
            // new site will be added.
            string strSitePath = args[0];
            SPSite site = new SPSite(strSitePath);
            
            // Add the new site (web)
            // ----------------------
            // Step 3: Add the site collection
            // args(0) = Site collection url
            // args(1) = Site (web) url
            // args(2) = Title
            // args(3) = Description
            // args(4) = Template
            SPWeb web = site.AllWebs.Add(args[1],args[2],args[3],(uint)1033,args[4],false,false);

            // Step 5: Confirm site (web) information
            Console.WriteLine();
            Console.WriteLine(("Site \'" + (args[0] + "\' successfuly created.")));
            Console.WriteLine();
            DisplayParams(args);

            // Release memory used by SPSite, SPWeb
            web.Dispose();
            site.Dispose();
        }
        catch (Exception ex)
        {
            // Step 6: If error occurs, display parameters and error message
            Console.WriteLine();
            Console.WriteLine("** ERROR OCCURRED **");
            Console.WriteLine();
            Console.WriteLine(ex.Message);
            Console.WriteLine();
            DisplayParams(args);
            Console.WriteLine();
        } 
    }

    private static void DisplayParams(string[] args)
    {
        try
        {
            // Step 7: Display parameters to console
            Console.WriteLine(("Site collection url: " + args[0]));
            Console.WriteLine(("Site (web): " + args[1]));
            Console.WriteLine(("Title: " + args[2]));
            Console.WriteLine(("Description: " + args[3]));
            Console.WriteLine(("Template: " + args[4]));
        }
        catch (Exception ex)
        {
            // If error occurred, display the error message
            Console.WriteLine();
            Console.WriteLine(ex.Message);
            Console.WriteLine();
        }
    }

    private static string[] GetParams(ref string[] args)
    {
        try
        {
            // Step 2: Get parameters from user
            Console.WriteLine();
            Console.Write("Site collection url: ");
            args[0] = Console.ReadLine();
            Console.Write("Site (web): ");
            args[1] = Console.ReadLine();
            Console.Write("Title: ");
            args[2] = Console.ReadLine();
            Console.Write("Description: ");
            args[3] = Console.ReadLine();
            Console.Write("Template: ");
            args[4] = Console.ReadLine();
        }
        catch (Exception ex)
        {
            // If an error occurred, display the error message
            Console.WriteLine();
            Console.WriteLine(ex.Message);
            Console.WriteLine();
        }
        return args;
    }
}