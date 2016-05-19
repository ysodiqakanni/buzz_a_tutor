using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;

namespace bat.logic.ViewModels.Records
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


                var parentLessons = conn.Lessons
                    .Join(conn.LessonParticipants,
                        l => l.ID, // lesson primary key
                        p => p.Lesson_ID, // equals participant foreign key
                        (l, p) => new {l, p})
                    .Select(s => new
                    {
                        s.l,
                        participant = s.p.Account_ID
                    })
                    .Where(l => l.participant == id);

                foreach (var l in parentLessons.ToList())
                {
                    this.lessons.Add(l.l);
                }

                //this.lessons = conn.Lessons.Where(p => p.Account_ID == id).ToList();              
            }
        }
    }
}