using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;

namespace bat.logic.ViewModels.Profile
{
    public class Profile : Master
    {
        public void Load(int id)
        {
            this.familyMembers = new List<FamilyMember>();
            using (var conn = new dbEntities())
            {
                this.account = conn.Accounts.FirstOrDefault(a => a.ID == id);
                if (this.account == null) throw new Exception("Account does not exist.");

                this.familyMembers = conn.FamilyMembers.Where(i => i.Parent_ID == id).ToList();
            }
        }

        public static string GetProfilePicture(int accountID)
        {
            using (var conn = new dbEntities())
            {
                var account = conn.Accounts.FirstOrDefault(a => a.ID == accountID);
                if (account == null)
                    throw new Exception("Account not found.");

                return account.Picture;
            }
        }
    }
}