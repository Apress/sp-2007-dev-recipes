using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;

namespace InstallSetDocumentFields_CS
{
    class Program
    {
        static void Main(string[] args)
        {

            SPSite site = new SPSite("http://mgerow-moss-vpc");
            SPWeb web = site.AllWebs["EventHandlers"];
            SPList tasks = web.Lists["Tasks"];

            int i;
            for (i = tasks.EventReceivers.Count - 1; i >= 0; i += -1)
            {
                tasks.EventReceivers[i].Delete();
            }

            //Add the variables event handler 
            string asmName = "SendEmail-CS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6b31e633957e0e4d";
            string className = "SendEmail_CS.SendEmail";
            tasks.EventReceivers.Add(SPEventReceiverType.ItemUpdated, asmName, className);
            tasks.EventReceivers[0].Data = "My subject line;My body text";
            tasks.Update();

        }

    }
}