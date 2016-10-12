using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;

class Module1 {
    
    static void Main(string[] args) {
        // Steps 1-2: If no arguments have been passed, prompt user to enter them now
        if ((args.Length == 0)) {
            args = new string[7];
            args = GetParams(ref args);
        }
        // Add the new site collection using the provided arguments
        AddSiteCollection(args);
    }
    
    private static int AddSiteCollection(string[] args) {
        try {
            // Get a handle to the root site on server
            string[] arrSitePath = args[0].Split('/');
            string strServerPath = (arrSitePath[0] + ("//" + arrSitePath[2]));
            SPSite site = new SPSite(strServerPath);
        
            // Get the list of site collections for the web application
            SPSiteCollection siteCollection = site.WebApplication.Sites;
            // Step 3: Add the site collection
            // args(0) = Site url
            // args(1) = Title 
            // args(2) = Description
            // args(3) = Web template
            // args(4) = Owner login
            // args(5) = Owner name
            // args(6) = Owner email
            
            siteCollection.Add(args[0], args[1], args[2], 1033, args[3], args[4], args[5], args[6]);
            // Step 5: Confirm site collection information
            Console.WriteLine();
            Console.WriteLine(("Site collection \'" 
                            + (args[0] + "\' successfuly created.")));
            Console.WriteLine();
            DisplayParams(args);

            // Release memory used by SPSite object
            site.Dispose();

            return 0;
        }
        catch (Exception ex) {
            // Step 6: If error occurs, display parameters and error message
            Console.WriteLine();
            Console.WriteLine("** ERROR OCCURRED **");
            Console.WriteLine();
            Console.WriteLine(ex.Message);
            Console.WriteLine();
            DisplayParams(args);
            Console.WriteLine();
            return -1;
        }
        Console.WriteLine("");
    }
    
    private static void DisplayParams(string[] args) {
        try {
            // Step 7: Display parameters to console
            Console.WriteLine(("Site url: " + args[0]));
            Console.WriteLine(("Title: " + args[1]));
            Console.WriteLine(("Description: " + args[2]));
            Console.WriteLine(("Template name: " + args[3]));
            Console.WriteLine(("Owner login: " + args[4]));
            Console.WriteLine(("Owner name: " + args[5]));
            Console.WriteLine(("Owner email: " + args[6]));
        }
        catch (Exception ex) {
            // If error occurred, display the error message
            Console.WriteLine();
            Console.WriteLine(ex.Message);
            Console.WriteLine();
        }
    }
    
    private static string[] GetParams(ref string[] args) {
        try {
            // Step 2: Get parameters from user
            Console.WriteLine();
            Console.Write("Site url: ");
            args[0] = Console.ReadLine();
            Console.Write("Title: ");
            args[1] = Console.ReadLine();
            Console.Write("Description: ");
            args[2] = Console.ReadLine();
            Console.Write("Template name: ");
            args[3] = Console.ReadLine();
            Console.Write("Owner login: ");
            args[4] = Console.ReadLine();
            Console.Write("Owner name: ");
            args[5] = Console.ReadLine();
            Console.Write("Owner email: ");
            args[6] = Console.ReadLine();
        }
        catch (Exception ex) {
            // If an error occurred, display the error message
            Console.WriteLine();
            Console.WriteLine(ex.Message);
            Console.WriteLine();
        }
        return args;
    }
}