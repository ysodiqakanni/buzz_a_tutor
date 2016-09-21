using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using bat.data;

namespace bat.logic.ViewModels.Homepage
{
    public class ForgotPassword
    {
        public void SetToken(string username, string resetPasswordUrl)
        {
            username = (username ?? "").Trim();

            using (var conn = new dbEntities())
            {
                var acc =
                    conn.Accounts.FirstOrDefault(
                        a => a.Email.Equals(username, StringComparison.CurrentCultureIgnoreCase));
                if (acc == null)
                    return;

                acc.ChangePasswordToken = Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "");
                acc.ChangePasswordTokenExpiry = DateTime.UtcNow.AddDays(1);

                conn.SaveChanges();

                Helpers.Emailer.SendPlainText(
                    "info@buzzatutor.com", "Buzz a Tutor",
                    acc.Email,
                    "Buzz a Tutor - Reset your password",
                    "Please click on the following link to reset your password." +
                        Environment.NewLine + resetPasswordUrl + "?t=" + acc.ChangePasswordToken);
            }
        }
    }
}
