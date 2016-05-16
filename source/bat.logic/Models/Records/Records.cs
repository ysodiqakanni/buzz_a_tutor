using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using bat.logic.Constants;
using bat.logic.Exceptions;

namespace bat.logic.Models.Records
{
    public class Records : Master
    {
        public Lesson lesson { get; set; }
        public List<Lesson> lessons { get; set; }

        public void Load(int id)
        {
            using (var conn = new dbEntities())
            {
                this.account = conn.Accounts.FirstOrDefault(a => a.ID == id);
                if (this.account == null) throw new Exception("Account does not exist.");

                this.lessons = conn.Lessons.Where(p => p.Account_ID == id).ToList();              
            }
        }
    }
}