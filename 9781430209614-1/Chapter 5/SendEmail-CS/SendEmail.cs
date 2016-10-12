using System.Web;
using Microsoft.SharePoint;

namespace SendEmail_CS
{
    public class SendEmail : SPItemEventReceiver
    {
        private SPItemEventProperties _properties;

        public override void ItemUpdated(Microsoft.SharePoint.SPItemEventProperties properties)
        {
            base.ItemUpdated(properties);

            _properties = properties;
            SPSecurity.RunWithElevatedPrivileges(emailIfComplete);

        }

        private void emailIfComplete()
        {

            //Step 1: Get a handle to the task that 
            // raised the update event 
            SPListItem item = _properties.ListItem;

            //Step 2: Determine if email should be sent based 
            // on current status of task. If so... 
            if (item["Status"] == "Completed")
            {
                _SendEmail(
                    "mgerow@fenwick.com", 
                    "tasklist@fenwick.com", 
                    "Task '" + item["Title"].ToString() + "' is Complete", 
                    "The task was marked complete by '" 
                        + _properties.UserDisplayName.ToString() 
                        + "'.", 
                    "SVCTA2.firm.fenwick.llp");
            }

        }

        private void _SendEmail(string Recipient, string Sender, string Subject, string Message, string Server)
        {

            //Step 3: Create message and SMTP client objects 
            System.Net.Mail.MailMessage msg;
            System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient(Server);

            //Step 4: Construct and send the message 
            msg = new System.Net.Mail.MailMessage(Sender, Recipient, Subject, Message);
            msg.IsBodyHtml = true;
            smtpClient.Send(msg);

        }
    }
}