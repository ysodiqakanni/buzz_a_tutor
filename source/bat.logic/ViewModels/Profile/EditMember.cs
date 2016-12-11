using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using System.Web.Mvc;
using bat.logic.Exceptions;
using EntityFramework.Extensions;

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
                if (this.familyMember == null) throw new Exception("Family member does not exist.");
                if (this.familyMember.Parent_ID != this.account.ID) throw new WrongAccountException();
            }
        }

        public void Save(FormCollection frm)
        {
            int memberID = Convert.ToInt32((frm["MemberID"]));
            using (var conn = new dbEntities())
            {
                this.familyMember = conn.FamilyMembers.FirstOrDefault(a => a.ID == memberID);
                if (this.familyMember == null) throw new Exception("Family Member does not exist.");

                var accountDetails = conn.Accounts.FirstOrDefault(a => a.ID == this.familyMember.Account_ID);
                if (accountDetails == null) throw new Exception("Account does not exist.");

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

                accountDetails.Fname = firstName;
                accountDetails.Lname = lastName;
                accountDetails.Email = email;         
                conn.SaveChanges();
            }
        }

        public void Delete(int id) {
            using (var conn = new dbEntities())
            {
                this.familyMember = conn.FamilyMembers.FirstOrDefault(a => a.ID == id);
                if (this.familyMember == null) throw new Exception("Family member does not exist.");
                if (this.familyMember.Parent_ID != this.account.ID) throw new WrongAccountException();

                conn.FamilyMembers.Remove(conn.FamilyMembers.FirstOrDefault(i => i.ID == id));
                conn.Accounts.Where(a => a.ID == this.familyMember.Account_ID).Delete();
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
