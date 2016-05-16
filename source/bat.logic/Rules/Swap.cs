using bat.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Rules
{
    public class Swap
    {
        public Account account { get; set; }

        public void Load(Account parent, int swapToUserId)
        {
            using (var conn = new dbEntities())
            {
                var owner = conn.Accounts.FirstOrDefault(a => a.ID == parent.ID);
                if (owner == null) throw new Exception("Parent does not exist.");

                this.account = conn.Accounts.FirstOrDefault(a => a.ID == swapToUserId);
                if (this.account == null) throw new Exception("Person does not exist.");

                if (!conn.FamilyMembers.Any(f => f.Parent_ID == owner.ID && f.Account_ID == this.account.ID))
                    throw new Exception("Person does not belong to this family.");
            }
        }
    }
}
