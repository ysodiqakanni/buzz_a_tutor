using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using bat.logic.Constants;
using bat.logic.Exceptions;

namespace bat.logic.ViewModels.Homepage
{
    public class Dashboard : Master
    {
        public List<Lesson> lessons { get; set; }

        public class classroom
        {
            public Lesson lesson { get; set; }
            public List<LessonParticipant> participants { get; set; } 
        }

        public List<classroom> classrooms { get; set; }

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
                                .OrderBy(p => p.Subject)
                                .ToList();
                        break;

                    case Types.AccountTypes.Teacher:
                        this.classrooms = new List<classroom>();
                        this.lessons = conn.Lessons.Where(l => l.Account_ID == this.account.ID)
                            .ToList();

                        foreach (var lesson in this.lessons)
                        {
                            if (lesson.BookingDate.Date >= DateTime.Now.Date && lesson.BookingDate.Date <= DateTime.Now.Date.AddDays(1))
                            {
                                this.classrooms.Add(new classroom
                                {
                                    lesson = lesson,
                                    participants = conn.LessonParticipants.Where(l => l.Lesson_ID == lesson.ID)
                                    .ToList()
                                }
                                );
                            }
                        }
                        break;
                }
            }
        }
    }
}
