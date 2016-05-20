using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;

namespace bat.logic.ViewModels.ChatRecords
{
    public class Index : Master
    {
        public Lesson lesson { get; set; }
        public List<Lesson> lessons { get; set; }
        public List<Lesson> familyLessons { get; set; }
        public List<Lesson> teacherLessons { get; set; }
        public class FamilyRecord
        {
            public Account familyMember { get; set; }
            public List<Lesson> lessons { get; set; }
        }
        
        public List<FamilyRecord> familyRecords { get; set; } 

        public void Load(int id)
        {
            using (var conn = new dbEntities())
            {
                this.account = conn.Accounts.FirstOrDefault(a => a.ID == id);
                if (this.account == null) throw new Exception("Account does not exist.");

                this.lessons = new List<Lesson>();
                this.familyRecords = new List<FamilyRecord>();

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

                this.teacherLessons = conn.Lessons.Where(p => p.Account_ID == id).ToList();

                this.familyMembers = conn.FamilyMembers.Where(p => p.Parent_ID == id).ToList();

                foreach (var familyMember in this.familyMembers.Select(a => new { a.Account_ID }).ToList())
                {
                    var lessons = new List<Lesson>();

                    var account = conn.Accounts.FirstOrDefault(a => a.ID == familyMember.Account_ID);
                    if (account == null) throw new Exception("Account does not exist.");

                    var childLessons = conn.Lessons
                        .Join(conn.LessonParticipants,
                            l => l.ID, // lesson primary key
                            p => p.Lesson_ID, // equals participant foreign key
                            (l, p) => new { l, p })
                        .Select(s => new
                        {
                            s.l,
                            participant = s.p.Account_ID
                        })
                        .Where(l => l.participant == familyMember.Account_ID);

                    foreach (var l in childLessons.ToList())
                    {
                        lessons.Add(l.l);
                    }

                    this.familyRecords.Add(new FamilyRecord()
                    {
                        familyMember = account,
                        lessons = lessons
                    });
                }
            }
        }
    }
}