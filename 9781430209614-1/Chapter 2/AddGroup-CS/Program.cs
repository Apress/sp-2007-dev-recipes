using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;

class Module1
{

    static public void Main(string[] args)
    {

        //Step 1: If no arguments passed in, prompt for them now 
        if ((args.Length == 0))
        {
            args = new string[7];
            args = GetParams(args);
        }

        try
        {

            //Step 3a: Get handle to specifie site collection and web site 
            SPSite site = new SPSite(args[0]);
            SPWeb web = site.AllWebs[args[1]];

            //Step 3b: Add the specified site group 
            web.SiteGroups.Add(args[2], web.SiteUsers[args[3]], web.SiteUsers[args[4]], args[5]);

            //Step 4: Assign specified role to the new group
            //Note: even though the SPWeb.Roles collection has been flagged as
            //  "obsolete" by Microsoft it is still the easieast way to 
            //  add roles to site groups in WSS/MOSS 2007
            web.Roles[args[6]].AddGroup(web.SiteGroups[args[2]]);

            //Step 5: Display success message 
            Console.WriteLine("Site group '" + args[2] + "' has been successfully added");
        }

        catch (Exception ex)
        {

            //Step 6: If error occurred, display error message 
            Console.WriteLine(ex.Message);

        }

        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.Read();

    }

    static private string[] GetParams(string[] args)
    {
        try
        {

            // Step 2: Get parameters from user 
            Console.WriteLine();
            Console.Write("Site collection url: ");
            args[0] = Console.ReadLine();
            Console.Write("Site (web): ");
            args[1] = Console.ReadLine();
            Console.Write("Site group to add: ");
            args[2] = Console.ReadLine();
            Console.Write("Owner login: ");
            args[3] = Console.ReadLine();
            Console.Write("Default user: ");
            args[4] = Console.ReadLine();
            Console.Write("Site group description: ");
            args[5] = Console.ReadLine();
            Console.Write("Role (Full Control/Contribute/Read): ");
            args[6] = Console.ReadLine();
            Console.WriteLine();
            Console.WriteLine();
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