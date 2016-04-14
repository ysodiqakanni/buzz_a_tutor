using bat.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Models.Lessons
{
    public class Leave : Master
    {
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
