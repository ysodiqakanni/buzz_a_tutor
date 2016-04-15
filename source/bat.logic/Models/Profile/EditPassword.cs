using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using bat.logic.Constants;
using bat.logic.Exceptions;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using bat.logic.Models;

namespace bat.logic.Models.Profile
{
    public class EditPassword : Master
    {
        public new Account account { get; set; }

        public void Load(int id)
        {
            using (var conn = new dbEntities())
            {
                this.account = conn.Accounts.FirstOrDefault(a => a.ID == id);
                if (this.account == null) throw new Exception("Account does not exist.");
            }
        }

        public void Save(int id, FormCollection frm)
        {
            using (var conn = new dbEntities())
            {
                account = conn.Accounts.FirstOrDefault(a => a.ID == id);
                if (this.account == null) throw new Exception("Account does not exist.");

                var password = (frm["Password"]);
                var confirmPassword = (frm["ConfirmPassword"]);

                bat.Logic.Rules.PasswordValidation.Match(password, confirmPassword);

                //Validation is turned off for dev.
                //bat.Logic.Rules.PasswordValidation.Validate(password);
                account.Password = Helpers.PasswordStorage.CreateHash(password);
                conn.SaveChanges();
            }
        }
    }
}



