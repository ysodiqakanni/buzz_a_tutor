using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using bat.logic.Constants;
using bat.logic.Exceptions;

namespace bat.logic.Models.Homepage
{
    public class Dashboard : Master
    {
        public List<Lesson> lessons { get; set; }
        public List<Lesson> lessonsToJoin { get; set; } 

        public void Load()
        {
            if (!this.initialised) throw new MasterModelNotInitialised();

            using (var conn = new dbEntities())
            {
                switch (this.accountType)
                {
                    case Types.AccountTypes.Student:
                        this.lessons =
                            conn.LessonParticipants.Where(p => p.Account_ID == this.account.ID)
                                .Select(p => p.Lesson)
                                .ToList();

                        var lessonIds = this.lessons.Select(ml => ml.ID);

                        this.lessonsToJoin =
                            conn.Lessons
                                .Where(l => !lessonIds.Contains(l.ID) &&
                                            l.ClassSize > l.LessonParticipants.Count).ToList();
                        break;

                    case Types.AccountTypes.Teacher:
                        this.lessons = conn.Lessons.Where(l => l.Account_ID == this.account.ID).ToList();
                        break;
                }
            }
        }
    }
}
