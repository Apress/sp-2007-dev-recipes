Module Module1

    'Define structure to hold user info
    Private Structure UserArgs
        Dim UserLogin As String
        Dim UserName As String
        Dim Email As String
        Dim UserDescription As String
        Dim GroupName As String
        Dim SiteUrl As String
        Dim WebName As String
    End Structure

    'Define structure to hold group info
    Private Structure GroupArgs
        Dim OwnerLogin As String
        Dim OwnerType As String
        Dim DefaultUserLogin As String
        Dim RoleName As String
        Dim Description As String
    End Structure

    Sub Main()

        'Step 1: Prompt for user info 
        Dim userArgs As UserArgs = GetUserArgs()

        'Step 2: Create an instance of the UserGroup web service class
        '   and set it's url to the target site/web
        Dim objUserGroup As New UserGroupService.UserGroup
        objUserGroup.Url = userArgs.SiteUrl & "/" & userArgs.WebName & "/_vti_bin/UserGroup.asmx"
        objUserGroup.Credentials = System.Net.CredentialCache.DefaultCredentials

        'Attemp to add user to group, if error occurs will need to get
        'some additional info about the group too
        Try

            'Step 3: Try adding user to the target group
            objUserGroup.AddUserToGroup( _
                userArgs.GroupName, _
                userArgs.UserName, _
                userArgs.UserLogin, _
                userArgs.Email, _
                userArgs.UserDescription)

        Catch exAddUser As Exception

            Try

                'Step 5: Initial user add attempt failed, 
                '   so try to create the group
                Dim groupArgs As GroupArgs = GetGroupArgs()

                objUserGroup.AddGroup( _
                    userArgs.GroupName, _
                    groupArgs.OwnerLogin, _
                    groupArgs.OwnerType, _
                    groupArgs.DefaultUserLogin, _
                    groupArgs.Description)

                'Step 6: Assign the new group to desired role
                objUserGroup.AddGroupToRole( _
                    groupArgs.RoleName, _
                    userArgs.GroupName)

                'Step 7: Try adding user again now that group exists
                objUserGroup.AddUserToGroup( _
                    userArgs.GroupName, _
                    userArgs.UserName, _
                    userArgs.UserLogin, _
                    userArgs.Email, _
                    userArgs.UserDescription)

            Catch exAddGroup As Exception

                Console.WriteLine(exAddGroup.Message)

            End Try

        End Try

        'Step 4: Display success message
        Console.WriteLine("Successfully added user '" & _
            userArgs.UserLogin & _
            "' to group '" & _
            userArgs.GroupName & "'")

        Console.WriteLine()
        Console.WriteLine("Press any key to continue...")
        Console.Read()

    End Sub

    Private Function GetGroupArgs() As GroupArgs
        Dim groupArgs As New GroupArgs
        Try
            Console.WriteLine()
            Console.Write("Group owner login: ")
            groupArgs.OwnerLogin = Console.ReadLine()
            Console.Write("Group default user login: ")
            groupArgs.DefaultUserLogin = Console.ReadLine()
            Console.Write("Owner login type (User/Group): ")
            groupArgs.OwnerType = Console.ReadLine()
            Console.Write("Group role (Full Control/Contribute/Read): ")
            groupArgs.RoleName = Console.ReadLine()
            Console.Write("Group description: ")
            groupArgs.Description = Console.ReadLine()
            Console.WriteLine()

        Catch ex As Exception
            ' If an error occurred, display the error message 
            Console.WriteLine()
            Console.WriteLine(ex.Message)
            Console.WriteLine()
        End Try

        Return groupArgs

    End Function
    Private Function GetUserArgs() As UserArgs
        Dim userArgs As New UserArgs
        Try

            ' Step 2: Get parameters from user 
            Console.WriteLine()
            Console.Write("Site collection url: ")
            userArgs.SiteUrl = Console.ReadLine()
            Console.Write("Site (web): ")
            userArgs.WebName = Console.ReadLine()
            Console.Write("Site group to add user to: ")
            userArgs.GroupName = Console.ReadLine()
            Console.Write("User login: ")
            userArgs.UserLogin = Console.ReadLine()
            Console.Write("User name: ")
            userArgs.UserName = Console.ReadLine()
            Console.Write("User email: ")
            userArgs.Email = Console.ReadLine()
            Console.Write("Description: ")
            userArgs.UserDescription = Console.ReadLine()
            Console.WriteLine()

        Catch ex As Exception
            ' If an error occurred, display the error message 
            Console.WriteLine()
            Console.WriteLine(ex.Message)
            Console.WriteLine()
        End Try

        Return userArgs

    End Function

End Module
