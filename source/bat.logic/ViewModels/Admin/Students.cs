using bat.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.ViewModels.Admin
{
    public class Students : Master
    {
        public List<Account> students { get; set; }
        public Account student { get; set; }

        public void Load()
        {
            using (var conn = new dbEntities())
            {
                this.students = conn.Accounts.Where(l => l.AccountType_ID == 1)
                    .ToList();
            }
        }

        public void Save(int id, bool status)
        {
            using (var conn = new dbEntities())
            {
                student = conn.Accounts.FirstOrDefault(l => l.ID == id);
                if (this.student == null) throw new Exception("Account does not exist.");

                student.Disabled = status;
                conn.SaveChanges();
            }
        }
    }
}
