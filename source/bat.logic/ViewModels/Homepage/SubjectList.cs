﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using bat.logic.Constants;
using bat.logic.Exceptions;

namespace bat.logic.ViewModels.Homepage
{
    public class SubjectList : Master
    {
        public List<Lesson> lessons { get; set; }
        public String subject { get; set;}
        public Account tutor { get; set; }

        public class virtRoom
        {
            public Lesson lesson { get; set; }
            public Account tutor { get; set; }
        }

        public List<virtRoom> classList { get; set; }

        public void Load(string subject, int? accountId)
        {
            this.subject = subject.Replace("_", " ");
            this.classList = new List<virtRoom>();
            using (var conn = new dbEntities())
            {
                var rs = conn.Lessons
                            .Where(l => l.Subject == this.subject && 
                                    (l.ClassSize == 0 || l.ClassSize > l.LessonParticipants.Count));

                lessons = new List<Lesson>();

                if (accountId.HasValue)
                    lessons = rs.Where(l => l.LessonParticipants.All(p => p.Account_ID != accountId)).ToList();
                else
                    lessons = rs.ToList();              

                foreach (var lesson in lessons)
                {
                    lesson.BookingDate = logic.Rules.Timezone.ConvertFromUTC(lesson.BookingDate);

                    this.classList.Add(new virtRoom
                    {
                        lesson = lesson,
                        tutor = conn.Accounts.FirstOrDefault(t => t.ID == lesson.Account_ID)
                });
                }
            }
        }
    }
}
