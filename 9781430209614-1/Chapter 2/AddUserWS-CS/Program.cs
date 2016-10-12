using System;
using System.Collections.Generic;
using System.Text;

namespace AddUserWS_CS
{

    class Module1
    {

        //Define structure to hold user info 
        private struct UserArgs
        {
            public string UserLogin;
            public string UserName;
            public string Email;
            public string UserDescription;
            public string GroupName;
            public string SiteUrl;
            public string WebName;
        }

        //Define structure to hold group info 
        private struct GroupArgs
        {
            public string OwnerLogin;
            public string OwnerType;
            public string DefaultUserLogin;
            public string RoleName;
            public string Description;
        }

        public static void Main()
        {

            //Step 1: Prompt for user info 
            UserArgs userArgs = GetUserArgs();

            //Step 2: Create an instance of the UserGroup web service class 
            // and set it's url to the target site/web 
            UserGroupService.UserGroup objUserGroup = new UserGroupService.UserGroup();
            objUserGroup.Url = userArgs.SiteUrl + "/" + userArgs.WebName + "/_vti_bin/UserGroup.asmx";
            objUserGroup.Credentials = System.Net.CredentialCache.DefaultCredentials;

            //Attemp to add user to group, if error occurs will need to get 
            //some additional info about the group too 
            try
            {

                //Step 3: Try adding user to the target group 
                objUserGroup.AddUserToGroup(userArgs.GroupName, userArgs.UserName, userArgs.UserLogin, userArgs.Email, userArgs.UserDescription);
            }

            catch (Exception exAddUser)
            {

                try
                {

                    //Step 5: Initial user add attempt failed, 
                    // so try to create the group 
                    GroupArgs groupArgs = GetGroupArgs();

                    objUserGroup.AddGroup(userArgs.GroupName, groupArgs.OwnerLogin, groupArgs.OwnerType, groupArgs.DefaultUserLogin, groupArgs.Description);

                    //Step 6: Assign the new group to desired role 
                    objUserGroup.AddGroupToRole(groupArgs.RoleName, userArgs.GroupName);

                    //Step 7: Try adding user again now that group exists 
                    objUserGroup.AddUserToGroup(userArgs.GroupName, userArgs.UserName, userArgs.UserLogin, userArgs.Email, userArgs.UserDescription);
                }

                catch (Exception exAddGroup)
                {

                    Console.WriteLine(exAddGroup.Message);

                }

            }

            //Step 4: Display success message 
            Console.WriteLine("Successfully added user '" + userArgs.UserLogin + "' to group '" + userArgs.GroupName + "'");

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.Read();

        }

        private static GroupArgs GetGroupArgs()
        {
            GroupArgs groupArgs = new GroupArgs();
            try
            {
                Console.WriteLine();
                Console.Write("Group owner login: ");
                groupArgs.OwnerLogin = Console.ReadLine();
                Console.Write("Group default user login: ");
                groupArgs.DefaultUserLogin = Console.ReadLine();
                Console.Write("Owner login type (User/Group): ");
                groupArgs.OwnerType = Console.ReadLine();
                Console.Write("Group role (Full Control/Contribute/Read): ");
                groupArgs.RoleName = Console.ReadLine();
                Console.Write("Group description: ");
                groupArgs.Description = Console.ReadLine();
                Console.WriteLine();
            }

            catch (Exception ex)
            {
                // If an error occurred, display the error message 
                Console.WriteLine();
                Console.WriteLine(ex.Message);
                Console.WriteLine();
            }

            return groupArgs;

        }
        private static UserArgs GetUserArgs()
        {
            UserArgs userArgs = new UserArgs();
            try
            {

                // Step 2: Get parameters from user 
                Console.WriteLine();
                Console.Write("Site collection url: ");
                userArgs.SiteUrl = Console.ReadLine();
                Console.Write("Site (web): ");
                userArgs.WebName = Console.ReadLine();
                Console.Write("Site group to add user to: ");
                userArgs.GroupName = Console.ReadLine();
                Console.Write("User login: ");
                userArgs.UserLogin = Console.ReadLine();
                Console.Write("User name: ");
                userArgs.UserName = Console.ReadLine();
                Console.Write("User email: ");
                userArgs.Email = Console.ReadLine();
                Console.Write("Description: ");
                userArgs.UserDescription = Console.ReadLine();
                Console.WriteLine();
            }

            catch (Exception ex)
            {
                // If an error occurred, display the error message 
                Console.WriteLine();
                Console.WriteLine(ex.Message);
                Console.WriteLine();
            }

            return userArgs;

        }

    }
}