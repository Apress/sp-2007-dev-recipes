using System;
using System.Collections.Generic;
using System.Text;

namespace TestFileUploadService
{
    class Program
    {
        static void Main(string[] args)
        {
            // If 3 arguments have not been passed
            // on command line, prompt user for inputs
            // via console
            if (args.Length < 4)
            {
                args = new string[4];
                Console.Write("Full path of source file: ");
                args[0] = Console.ReadLine();
                Console.Write("Target site url: ");
                args[1] = Console.ReadLine();
                Console.Write("Target web name: ");
                args[2] = Console.ReadLine();
                Console.Write("Target folder path: ");
                args[3] = Console.ReadLine();
            }

            // Create an instance of the file upload service
            // and assign credentials of an authorized user
            FileUploadService_VB.Service objFileUploadService = new FileUploadService_VB.Service();
            objFileUploadService.Credentials = System.Net.CredentialCache.DefaultCredentials;

            // Read in source file
            System.IO.FileStream fs = new System.IO.FileStream(args[0],System.IO.FileMode.Open);
            byte[] fileContents = new byte[fs.Length];
            fs.Read(fileContents,0,(int)fs.Length);
            fs.Close();

            // Get just the file name
            string fileName = args[0].Split('\\')[args[0].Split('\\').Length-1];

            // Upload the specified file
            Console.WriteLine();
            Console.WriteLine(
                objFileUploadService.UploadFile2SharePoint(
                    fileName, 
                    fileContents, 
                    args[1], 
                    args[2], 
                    args[3])
                    );

            Console.WriteLine();
            Console.WriteLine("Press any key to continue");
            Console.Read();
        }

    }
}
