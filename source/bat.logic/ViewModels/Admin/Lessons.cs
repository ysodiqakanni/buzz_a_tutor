using bat.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.ViewModels.Admin
{
    public class Lessons : Master
    {
        public List<bat.data.Lesson> lessons { get; set; }
        public Account tutor { get; set; }

        public void Load()
        {
            using (var conn = new dbEntities())
            {
                this.lessons = conn.Lessons.Where(l => l.BookingDate >= DateTime.UtcNow)
                    .OrderBy(l => l.BookingDate)
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

        public void Approve(int id, bool status)
        {
            using (var conn = new dbEntities())
            {
                tutor = conn.Accounts.FirstOrDefault(l => l.ID == id);
                if (this.tutor == null) throw new Exception("Account does not exist.");

                tutor.Approved = status;
                conn.SaveChanges();
            }
        }
    }
}
