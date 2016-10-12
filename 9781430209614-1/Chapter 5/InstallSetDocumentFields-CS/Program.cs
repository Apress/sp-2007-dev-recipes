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
            SPList list = web.Lists["MyDocs"];

            // Remove any pre-existing event receivers
            for (int i = list.EventReceivers.Count - 1; i > -1; i--)
            {
                list.EventReceivers[i].Delete();
            }

            // Add the new event receiver
            string asmName = "SetDocumentFields-CS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=453cdb407c59801a";
            string className = "SetDocumentFields_CS.SetDocumentFields";
            list.EventReceivers.Add(SPEventReceiverType.ItemAdded, asmName, className);
        }
    }
}
