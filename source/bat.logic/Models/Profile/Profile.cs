using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using bat.logic.Constants;
using bat.logic.Exceptions;

namespace bat.logic.Models.Profile
{
    public class Profile : Master
    {
        public List<FamilyMember> familyMembers { get; set; }

        public void Load(int id)
        {
            this.familyMembers = new List<FamilyMember>();
            using (var conn = new dbEntities())
            {
                this.account = conn.Accounts.FirstOrDefault(a => a.ID == id);
                if (this.account == null) throw new Exception("Account does not exist.");

                this.familyMembers = conn.FamilyMembers.ToList();
            }
        }
    }
}