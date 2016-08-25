using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using bat.logic;

namespace bat.logic.ViewModels.Homepage
{
    public class TeacherList : Master
    {
        public List<Account> teachers { get; set; }

        public void Load(string subject)
        {
            using (var conn = new dbEntities())
            {
                //Note to add ability to search Teachers by subject.
                this.teachers = conn.Accounts
                            .Where(l => l.AccountType_ID == Constants.Types.Teacher && (l.Approved != false && l.Disabled != true))
                            .ToList();               
            }
        }
    }
}
    