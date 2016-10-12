using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;

namespace AddRoleOM_CS
{
    class Program
    {

        private struct roleData
        {
            public string SiteUr;
            public string WebName;
            public string RoleName;
            public string RoleDescription;
            public SPBasePermissions PermissionMask;            
        }

        static public void Main()
        {

            //Step 1: Prompt data needed to define
            //  the new role
            roleData r = GetParams();

            try
            {

                //Step 2: Get handle to specifie site collection and web site 
                SPSite site = new SPSite(r.SiteUr);
                SPWeb web = site.AllWebs[r.WebName];
                web.AllowUnsafeUpdates = true;

                //Step 3: Get collection of current role definitions for site
                SPRoleDefinitionCollection roles = web.RoleDefinitions;
                
                //Step 4: Create a new role using information passed in
                SPRoleDefinition role = new SPRoleDefinition();
                role.Name = r.RoleName;
                role.Description = r.RoleDescription;
                role.BasePermissions = r.PermissionMask;
                roles.Add(role);

                //Step 5: Display success message 
                Console.WriteLine("Role '" + r.RoleName + "' has been successfully added");
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

        static private roleData GetParams()
        {
            roleData r = new roleData();
            
            try
            {
                // Get the basic data 
                Console.WriteLine();
                Console.Write("Site collection url: ");
                r.SiteUr = Console.ReadLine();
                Console.Write("Site (web): ");
                r.WebName = Console.ReadLine();
                Console.Write("Role to add: ");
                r.RoleName = Console.ReadLine();
                Console.Write("Description of role: ");
                r.RoleDescription = Console.ReadLine();

                // Now get a character that represents the 
                // set of permissions the new role should
                // inherit
                Console.Write("Role (F=Full Control, C=Contribute, R=Read): ");
                string strBasePermission = Console.ReadLine();
                
                //Only allow user to enter valid permission character,
                //keep looping until valid response provided
                SPSite site = new SPSite(r.SiteUr);
                SPWeb web = site.AllWebs[r.WebName];
                r.PermissionMask = SPBasePermissions.EmptyMask;
                while (true)
                {
                    switch (strBasePermission.ToUpper())
                    {
                        case "F":
                            r.PermissionMask = web.RoleDefinitions["Full Control"].BasePermissions;
                            break;
                        case "C":
                            r.PermissionMask = web.RoleDefinitions["Contribute"].BasePermissions;
                            break;
                        case "R":
                            r.PermissionMask = web.RoleDefinitions["Read"].BasePermissions;
                            break;
                    }
                    if (r.PermissionMask != SPBasePermissions.EmptyMask)
                        break;
                    else
                    {
                        Console.Write("Your selection was not valid (F=Full Control, C=Contribute, R=Read): ");
                        strBasePermission = Console.ReadLine();
                    }
                }

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

            return r;

        }

    }
}
