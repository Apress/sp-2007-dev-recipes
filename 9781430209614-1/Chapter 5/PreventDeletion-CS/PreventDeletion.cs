using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;

namespace PreventDeletion_CS
{
    public class PreventDeletion : SPItemEventReceiver
    {
        private SPItemEventProperties _properties;
        
        public override void ItemDeleting(SPItemEventProperties properties)
        {
            base.ItemDeleting(properties);

            _properties = properties;
            SPSecurity.RunWithElevatedPrivileges(deleteItem);
        }

        private void deleteItem()
        {
            // Step 1: get handle to list item that raised 
            //  the delete event
            SPListItem item = _properties.ListItem;

            // Step 2: get object representing the user
            //  attempting to delete the task
            SPWeb web = _properties.ListItem.ParentList.ParentWeb;
            SPUser user = web.Users[_properties.UserLoginName];

            // Step 3: determine whether task can
            //  be deleted
            bool ok2Delete =
                (item["Status"].ToString() == "Not Started" && float.Parse(item["% Complete"].ToString()) == 0)
                || user.IsSiteAdmin
                || _properties.UserDisplayName == "System Account";

            // Step 4: if task is in progress and user
            //  requesting deletion isn't a site administrator
            //  or the "System Account", prevent the
            //  deletion.
            if (!ok2Delete)
            {
                _properties.Cancel = true;
                _properties.ErrorMessage = "Unable to delete task '" + item["Title"].ToString() + "'.  Only site administrators may delete tasks on which work has already begun.";
            }
        }
    }
}
