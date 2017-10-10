using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace bat.logic.Helpers
{
    class Emailer
    {
        public static void SendPlainText(string fromAddress, string fromName, string recipient, string subject, string body)
        {
            var from = new EmailAddress(fromAddress, fromName);
            var recipientEmail = new EmailAddress(recipient);
            var msg = MailHelper.CreateSingleEmail(from, recipientEmail, subject, body, body);
            
            var sg = new SendGridClient(Constants.SendGrid.Key);
            var rs = sg.SendEmailAsync(msg).Result;
        }
    }
}
