using bat.data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace bat.logic.ViewModels.Lessons
{
    public class Join : Master
    {
        public Lesson lesson { get; set; }

        public Join()
        {
            this.lesson = new Lesson()
            {
                BookingDate = Shearnie.Net.OzTime.GetNowAEST(),
                DurationMins = 15,
                ClassSize = 0,
                TokBoxSessionId = "",
                ZoomStartUrl = "",
                ZoomJoinUrl = ""
            };
        }

        public bool IsTeacher =>
            this.account.AccountType_ID == (int)bat.logic.Constants.Types.AccountTypes.Teacher;

        public bool Load(int id)
        {
            using (var conn = new dbEntities())
            {
                this.lesson = conn.Lessons.FirstOrDefault(l => l.ID == id);
                if (this.lesson == null) throw new Exception("Lesson does not exist.");

                if (this.lesson.ClassSize <= 0) return true;

                return this.lesson.LessonParticipants.Count() < this.lesson.ClassSize;
            }
        }

        public void Save()
        {
            using (var conn = new dbEntities())
            {
                if (conn.LessonParticipants.Any(l => l.Account_ID == this.account.ID && l.Lesson_ID == this.lesson.ID))
                    return;

                if (this.lesson.ClassSize > 0 && 
                    conn.LessonParticipants.Count(l => l.Lesson_ID == this.lesson.ID) >= this.lesson.ClassSize)
                    throw new Exception("Class is full.");

                conn.LessonParticipants.Add(new LessonParticipant()
                {
                    Account_ID = this.account.ID,
                    Lesson_ID = this.lesson.ID
                });
                conn.SaveChanges();
            }
        }
    }
}
