using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
namespace bat.logic.ViewModels.Admin
{
    public class Lesson : Master
    {
        public bat.data.Lesson lesson { get; set; }
        public List<LessonParticipant> participants { get; set; }
        public void Load (int id)
        {
            using (var conn = new dbEntities())
            {
                this.lesson = conn.Lessons.FirstOrDefault(l => l.ID == id);
                if (lesson == null)
                    throw new Exception("Lesson not found");

                this.participants = conn.LessonParticipants.Where(p => p.Lesson_ID == id)
                    .ToList();
            }
        }

        public void LessonVisibility(int id, bool status)
        {
            using (var conn = new dbEntities())
            {
                lesson = conn.Lessons.FirstOrDefault(l => l.ID == id);
                if (this.lesson == null) throw new Exception("Account does not exist.");

                lesson.Hidden = status;
                conn.SaveChanges();
            }
        }
    }
}
