using bat.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.ViewModels.Lessons
{
    public class Leave : Master
    {
        public Lesson lesson { get; set; }

        public bool Load(int id)
        {
            using (var conn = new dbEntities())
            {
                this.lesson = conn.Lessons.FirstOrDefault(l => l.ID == id);
                if (this.lesson == null) throw new Exception("Lesson does not exist.");

                // timezone out for displaying
                this.lesson.BookingDate = Rules.Timezone.ConvertFromUTC(this.lesson.BookingDate);

                return conn.LessonParticipants.Any(l => l.Account_ID == this.account.ID && l.Lesson_ID == this.lesson.ID);
            }
        }

        public void Delete(int lessonid, int studentid)
        {
            using (var conn = new dbEntities())
            {
                var participants = conn.LessonParticipants.Where(i => i.Lesson_ID == lessonid && i.Account_ID == studentid);
                conn.LessonParticipants.RemoveRange(participants);
                conn.SaveChanges();
            }
        }
    }
}
