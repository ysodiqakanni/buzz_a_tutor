using bat.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.ViewModels.Admin
{
    public class Tutors : Master
    {
        public List<Account> tutors { get; set; }
        public Account tutor { get; set; }

        public void Load()
        {
            using (var conn = new dbEntities())
            {
                this.tutors = conn.Accounts.Where(l => l.AccountType_ID == 2)
                    .ToList();
            }
        }

        public void Save(int id, bool status)
        {
            using (var conn = new dbEntities())
            {
                tutor = conn.Accounts.FirstOrDefault(l => l.ID == id);
                if (this.tutor == null) throw new Exception("Account does not exist.");

                tutor.Disabled = status;
                conn.SaveChanges();
            }
        }
    }
}
