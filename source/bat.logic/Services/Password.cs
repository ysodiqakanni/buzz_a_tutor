using bat.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using bat.logic.ViewModels.Homepage;

namespace bat.logic.Services
{
    public class Password : _ServiceClassBaseMarker
    {
        public void SetForgotPasswordToken(string username, string resetPasswordUrl)
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
                    Constants.Email.Address, Constants.Email.Name,
                    acc.Email,
                    "Buzz a Tutor - Reset your password",
                    "Please click on the following link to reset your password." +
                    Environment.NewLine + resetPasswordUrl + "?t=" + acc.ChangePasswordToken);
            }
        }

        public ResetPassword VerifyToken(string token)
        {
            using (var conn = new dbEntities())
            {
                var acc =
                    conn.Accounts.FirstOrDefault(
                        a => a.ChangePasswordToken.Equals(token, StringComparison.CurrentCultureIgnoreCase));
                if (acc == null)
                    throw new Exception("This password reset link is not valid.");

                if (acc.ChangePasswordTokenExpiry < DateTime.UtcNow)
                    throw new Exception("This password reset link has expired.");

                return new ResetPassword()
                {
                    token = token
                };
            }
        }

        public bat.data.Account ChangePassword(string token, string txtPassword1, string txtPassword2)
        {
            using (var conn = new dbEntities())
            {
                var acc =
                    conn.Accounts.FirstOrDefault(
                        a => a.ChangePasswordToken.Equals(token, StringComparison.CurrentCultureIgnoreCase));
                if (acc == null)
                    throw new Exception("This password reset link is not valid.");

                if (txtPassword1 != txtPassword2)
                    throw new Exception("Confirm password must match password.");

                acc.Password = Helpers.PasswordStorage.CreateHash(txtPassword1);

                conn.SaveChanges();

                return acc;
            }
        }
    }
}
