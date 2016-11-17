using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using System.Web.Mvc;

namespace bat.logic.ViewModels.Profile
{
    public class EditMember : Master
    {
        public FamilyMember familyMember { get; set; }

        public void load (int id)
        {
            using (var conn = new dbEntities())
            {
                this.familyMember = conn.FamilyMembers.FirstOrDefault(a => a.ID == id);
                if (this.account == null) throw new Exception("Family member does not exist.");
            }
        }

        public void Save(FormCollection frm)
        {
            int memberID = Convert.ToInt32((frm["MemberID"]));
            using (var conn = new dbEntities())
            {
                this.familyMember = conn.FamilyMembers.FirstOrDefault(a => a.ID == memberID);
                if (this.familyMember == null) throw new Exception("Family Member does not exist.");

                this.account = conn.Accounts.FirstOrDefault(a => a.ID == this.familyMember.Account_ID);
                if (this.account == null) throw new Exception("Account does not exist.");

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

                account.Fname = firstName;
                account.Lname = lastName;
                account.Email = email;         
                conn.SaveChanges();
            }
        }

        public void Delete(int id) {
            using (var conn = new dbEntities())
            {
                this.familyMember = conn.FamilyMembers.FirstOrDefault(a => a.ID == id);
                if (this.account == null) throw new Exception("Account does not exist.");

                conn.FamilyMembers.Remove(conn.FamilyMembers.FirstOrDefault(i => i.ID == id));
                conn.SaveChanges();
            }
        }

        public data.Account FamilyMemberAccount
        {
            get
            {
                using (var conn = new dbEntities())
                {
                    return conn.Accounts.FirstOrDefault(a => a.ID == this.familyMember.Account_ID) ?? new Account();
                }
            }
        }
    }
}
