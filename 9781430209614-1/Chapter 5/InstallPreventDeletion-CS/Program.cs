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
            // Get handle to target site, web and list
            SPSite site = new SPSite("http://mgerow-moss-vpc");
            SPWeb web = site.AllWebs["EventHandlers"];
            SPList list = web.Lists["Tasks"];

            // Remove any pre-existing event receivers
            for (int i = list.EventReceivers.Count - 1; i > -1; i--)
            {
                list.EventReceivers[i].Delete();
            }

            // Add the new event receiver
            string asmName = "PreventDeletion-CS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=236c1d4aa4b4d71f";
            string className = "PreventDeletion_CS.PreventDeletion";
            list.EventReceivers.Add(SPEventReceiverType.ItemDeleting, asmName, className);
        }
    }
}
