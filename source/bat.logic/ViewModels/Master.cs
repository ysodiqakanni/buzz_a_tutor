using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using bat.logic.Constants;
using bat.logic.Exceptions;

namespace bat.logic.ViewModels
{
    public class Master
    {
        internal bool initialised { get; set; }
        public Account account { get; set; }
        public Constants.Types.AccountTypes accountType => (Constants.Types.AccountTypes) this.account.AccountType_ID;
        public List<FamilyMember> familyMembers { get; set; }
        public Account parent { get; set; }
        public String name { get; set; }

        public Master()
        {
            this.initialised = false;
            this.account = new Account();
        }

        public void Initialise(int accountId)
        {
            if (accountId == 0)
            {
                this.account = new Account()
                {
                    ID = 0,
                    Email = AdminLogin.User
                };
                this.initialised = true;
                return;
            }

            using (var conn = new dbEntities())
            {
                this.account = conn.Accounts.FirstOrDefault(a => a.ID == accountId);
                if (this.account == null) throw new InvalidRecordException();

                if (this.account.AccountType_ID == Constants.Types.Teacher && !(this.account.Approved ?? false))
                    throw new Exception("Your account is pending approval.");

                if (this.account.Disabled ?? false)
                    throw new Exception("Your account has been disabled.");

                this.familyMembers = conn.FamilyMembers.Where(i => i.Parent_ID == accountId).ToList();

                // if not a parent, might be a family member
                if (!this.familyMembers.Any())
                {
                    var fam = conn.FamilyMembers.FirstOrDefault(i => i.Account_ID == accountId);
                    if (fam != null)
                        this.parent = conn.Accounts.FirstOrDefault(a => a.ID == fam.Parent_ID);
                }
            }
            this.initialised = true;
        }

        public void Initialise(Account account)
        {
            this.account = account;
            this.initialised = true;
        }

        public string GetAccountFirstName(int accountId)
        {
            using (var conn = new dbEntities())
            {
                return conn.Accounts.Select(a => new { a.ID, a.Fname }).FirstOrDefault(a => a.ID == accountId)?.Fname;
            }
        }

        public string GetFullAccountName(int accountId)
        {
            using (var conn = new dbEntities())
            {
                var fName = conn.Accounts.Select(a => new { a.ID, a.Fname }).FirstOrDefault(a => a.ID == accountId)?.Fname;
                var lName = conn.Accounts.Select(a => new { a.ID, a.Lname }).FirstOrDefault(a => a.ID == accountId)?.Lname;
                string name = fName + " " + lName;
                return name;
            }
        }

        public bool IsEnabled =>
            (this.account.Approved ?? false) == Constants.Status.Approved && (this.account.Disabled ?? false) == Constants.Status.Enabled;

        public bool IsTeacher =>
            this.account.AccountType_ID == (int) Constants.Types.AccountTypes.Teacher;

    }
}
