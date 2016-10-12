using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;

namespace InstallUpdateFieldsEventHandler_CS
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get handle to target site, web and list
            SPSite site = new SPSite("http://mgerow-moss-vpc");
            SPWeb web = site.AllWebs["EventHandlers"];
            SPList list = web.Lists["UpdateFieldsList"];

            // Remove any pre-existing event receivers
            for (int i = list.EventReceivers.Count-1; i > -1 ; i--)
            {
                list.EventReceivers[i].Delete();
            }

            // Add the new event receiver
            string asmName = "UpdateFieldsEventHandler-CS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=4b4bd299aaa7406b";
            string className = "UpdateFieldsEventHandler_CS.UpdateFieldsEventHandler";
            list.EventReceivers.Add(SPEventReceiverType.ItemAdded, asmName, className);
            list.EventReceivers.Add(SPEventReceiverType.ItemAdding, asmName, className);
            list.EventReceivers.Add(SPEventReceiverType.ItemUpdated, asmName, className);
        }
    }
}
