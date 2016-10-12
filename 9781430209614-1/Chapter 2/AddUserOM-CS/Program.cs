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
            args = new string[8];
            args = GetParams(args);
        }

        try
        {

            //Step 3a: Get handle to specifie site collection and web site 
            SPSite site = new SPSite(args[0]);
            SPWeb web = site.AllWebs[args[1]];

            //Step 3b: Add the user to the specified site group 
            web.SiteGroups[args[6]].AddUser(args[2], args[3], args[4], args[5]);

            //Step 4-5: Display error message 
            Console.WriteLine("User '" + args[2] + "' has been successfully added to site group '" + args[6] + "'");
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
            Console.Write("User login: ");
            args[2] = Console.ReadLine();
            Console.Write("Email address: ");
            args[3] = Console.ReadLine();
            Console.Write("User name: ");
            args[4] = Console.ReadLine();
            Console.Write("Notes: ");
            args[5] = Console.ReadLine();
            Console.Write("Site Group to add user to: ");
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