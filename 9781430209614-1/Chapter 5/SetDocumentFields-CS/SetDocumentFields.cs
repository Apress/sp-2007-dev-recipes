using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;

namespace SetDocumentFields_CS
{
    public class SetDocumentFields : SPItemEventReceiver
    {
        // Name of hidden item that contains default properties
        const string DEFAULT_FILE_TITLE = "[defaults]";

        // local varialble to contain data passed in from SharePoint
        private SPItemEventProperties  _properties;

        // Method called AFTER a new item has been added
        public override void ItemAdded(SPItemEventProperties properties)
        {
            base.ItemAdded(properties);
            _properties = properties;

            // Run under system account
            SPSecurity.RunWithElevatedPrivileges(setFields);
        }

        private void setFields()
        {
            // Step 1: Get a handle to the item that raised the event
            SPListItem item = _properties.ListItem;

            // Step 2: Get a handle to folder containing the 
            // document just uploaded
            SPFolder folder = null;
            try 
	        {
                if (item.File.ParentFolder != null)
                {
                    folder = item.File.ParentFolder;
                }
                else
                {
                    folder = item.ParentList.RootFolder;
                }
	        }
	        catch (Exception ex)
	        {
                // No op
	        }

            // Step 3: Assuming a folder was found (which
            // should be in all cases, find the associated
            // [defaults] document.
            if (folder != null)
            {
                SPFileCollection files = folder.Files;
                string client = "";
                string matter = "";

                // Step 4: Find the document containing defaults, and
                // use its "Client" and "Matter" values for 
                // new document
                foreach (SPFile file in files)
                {
                    if (file.Title.ToLower() == DEFAULT_FILE_TITLE)
                    {
                        client = file.Item["Client"].ToString();
                        matter = file.Item["Matter"].ToString();
                        break;
                    }
                }
                item["Client"] = client;
                item["Matter"] = matter;

                // Step 5: Save changes without updating the
                // MODIFIED, MODIFIED BY fields
                item.SystemUpdate(false);
            }
        }
    }
}
