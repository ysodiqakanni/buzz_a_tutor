using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Mail;

namespace bat.logic.Helpers
{
    class Emailer
    {
        public static void SendPlainText(string fromAddress, string fromName, string recipient, string subject, string body)
        {
            var mail = new SendGrid.Helpers.Mail.Mail(
                new SendGrid.Helpers.Mail.Email(fromAddress, fromName), 
                subject,
                new SendGrid.Helpers.Mail.Email(recipient),
                new SendGrid.Helpers.Mail.Content("text/plain", body)
            );

            dynamic sendGridClient = new SendGrid.SendGridAPIClient(Constants.SendGrid.Key, "https://api.sendgrid.com");
            dynamic response = sendGridClient.client.mail.send.post(requestBody: mail.Get());
        }
    }
}
