using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using System.Web.Mvc;
namespace bat.logic.ViewModels.Profile
{
    public class New : Master
    {
        public FamilyMember familyMemeber { get; set; }
        public Account Account { get; set; }

        public New()
        {
            this.familyMemeber = new FamilyMember();
            this.Account = new Account();
        }

        public void Save(int id, FormCollection frm)
        {
            using (var conn = new dbEntities())
            {
                var firstName = (frm["Fname"] ?? "").Trim();
                var lastName = (frm["Lname"] ?? "").Trim();
                var email = (frm["Email"] ?? "").Trim();

                if (string.IsNullOrEmpty(firstName))
                    throw new Exception("First name required.");

                if (string.IsNullOrEmpty(lastName))
                    throw new Exception("Last name required.");

                if (string.IsNullOrEmpty(email))
                    throw new Exception("Email required.");

                if (!Helpers.Strings.EmailValid(email))
                    throw new Exception("Email not valid.");

                var accountDetails = new Account()
                {
                    AccountType_ID = (int) Constants.Types.AccountTypes.Student,
                    Fname = firstName,
                    Lname = lastName,
                    Email = email,
                    Password = Helpers.PasswordStorage.CreateHash("")
                };
                conn.Accounts.Add(accountDetails);
                conn.SaveChanges();

                this.familyMemeber = new FamilyMember()
                {
                    Account_ID = accountDetails.ID,
                    Parent_ID = id,
                };
                conn.FamilyMembers.Add(this.familyMemeber);
                conn.SaveChanges();
            }
        }
    }
}



