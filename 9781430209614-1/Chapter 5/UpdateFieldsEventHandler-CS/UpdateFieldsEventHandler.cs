using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace UpdateFieldsEventHandler_CS
{
    public class UpdateFieldsEventHandler : SPItemEventReceiver
    {
        // Local veriable used to pass event properties to 
        // setFields() method.
        private SPItemEventProperties _properties;

        const string errMsg = "Rejected because order $Amt too large, please break into orders of less than $100,000.";

        // The ItemAdding method is called BEFORE the item is
        // written to the SharePoint content database.  If
        // this event is cancelled the item will be discarded.
        public override void ItemAdding(SPItemEventProperties properties)
        {
            base.ItemAdding(properties);
            _properties = properties;

            // Run as system account to avoid any permission issues
            Microsoft.SharePoint.SPSecurity.RunWithElevatedPrivileges(validateAddUpdate);
        }

        // The ItemAdding method is called AFTER the item has been
        // written to the content database.  Place code or method
        // calls here that will update fields or take action based
        // on data saved.
        public override void ItemAdded(SPItemEventProperties properties)
        {
            base.ItemAdded(properties);
            _properties = properties;

            // Run as system account to avoid any permission issues
            Microsoft.SharePoint.SPSecurity.RunWithElevatedPrivileges(setFields);
        }

        // The ItemUpdated method is called after changes are written
        // to the SharePoint content database.  Place code or method
        // calls here to update fields or take action based on values
        // written.
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            base.ItemUpdated(properties);
            _properties = properties;

            // Run as system account to avoid any permission issues
            Microsoft.SharePoint.SPSecurity.RunWithElevatedPrivileges(setFields);
        }

        // Validation has been passed, so apply business
        // rules to update the approval fields.
        private void setFields()
        {
            try
            {
                // Step 1: get a handle to the list item
                //  that raised this event
                SPListItem li = _properties.ListItem;

                // Step 2: Calculate the extended price
                float extPrice = float.Parse(li["Price"].ToString()) * float.Parse(li["Quantity"].ToString());

                // Step 3: Determine if approval is required
                bool approvalRequired = (extPrice > 5000);
                li["Extended Price"] = extPrice;
                li["Approval Required?"] = approvalRequired;

                // Step 4: If approval is required, assign to approver
                if (approvalRequired)
                {                   
                    // Step 5: Assigne default value to note field
                    li["Notes"] = "Approver assigned at " + DateTime.Now.ToString();

                    // Step 6: This is where the business logic gets applied.  
                    // Of course your business logic will likely be more
                    // complex, but the process is the same
                    if (extPrice < 10000) {
                        li["Approver"] = "Dept Mgr";
                    }
                    else {
                        if (extPrice < 25000) {
                            li["Approver"] = "CFO";
                        }
                        else {
                            if (extPrice < 100000) {
                                li["Approver"] = "President/CEO";
                            }
                            else {
                                li["Notes"] = errMsg;
                                li["Approver"] = "";
                                li["Approval Required?"] = false;
                                li["Quantity"] = 0;
                                li["Extended Price"] = 0;
                            }
                        }
                    }
                }

                // Step 7: update item, but don't reset 
                // the MODIFIED or MODIFIED BY fields.
                li.SystemUpdate();
            }
            catch (Exception ex)
            {
                // Handle error
            }
        }

        // This method checks to see if user has entered 
        // Price and/or Quantity that results in an
        // Extended Amount in excess of $100,000
        private void validateAddUpdate()
        {
            float extPrice = float.Parse(_properties.AfterProperties["Price"].ToString()) * float.Parse(_properties.AfterProperties["Quantity"].ToString());

            // Check to see if new item exceed extended price limit of $100,000
            if (extPrice > 99999 && _properties.EventType == SPEventReceiverType.ItemAdding)
            {
                _properties.Cancel = true;
                _properties.ErrorMessage = errMsg;
            }
        } 

    }
}
