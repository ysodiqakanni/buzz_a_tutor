using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using bat.data;

namespace bat.logic.ViewModels.Homepage
{
    public class ResetPassword
    {
        public string token { get; set; }

        public void VerifyToken(string token)
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

                this.token = token;
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

                this.token = token;

                if (txtPassword1 != txtPassword2)
                    throw new Exception("Confirm password must match password.");

                acc.Password = Helpers.PasswordStorage.CreateHash(txtPassword1);

                conn.SaveChanges();

                return acc;
            }
        }
    }
}
