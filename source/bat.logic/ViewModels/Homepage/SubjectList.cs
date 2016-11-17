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
        public string subject { get; set;}
        public SubjectDescription subjectDescription { get; set; }
        public List<SubjectExamPaper> ExamPapers { get; set; }
        public Account tutor { get; set; }

        public class virtRoom
        {
            public Lesson lesson { get; set; }
            public Account tutor { get; set; }
        }

        public List<virtRoom> classList { get; set; }

        public SubjectList()
        {
            this.lessons = new List<Lesson>();
            this.classList = new List<virtRoom>();
            this.ExamPapers = new List<SubjectExamPaper>();
            this.subjectDescription = new SubjectDescription();
            this.tutor = new Account();
        }

        public void Load(string subject, int? accountId)
        {
            this.subject = subject.Replace("_", " ");
            using (var conn = new dbEntities())
            {
                this.subjectDescription = conn.SubjectDescriptions.FirstOrDefault(s => s.Subject == this.subject) ??
                                          new SubjectDescription()
                                          {
                                              Subject = subject
                                          };
                this.ExamPapers = conn.SubjectExamPapers.Where(s => s.SubjectDescription_ID == this.subjectDescription.ID).ToList();

                var rs = conn.Lessons
                            .Where(l => l.Subject == this.subject && 
                                    (l.ClassSize == 0 || l.ClassSize > l.LessonParticipants.Count));

                lessons = new List<Lesson>();
                var tutors = new List<Account>();
                var teacherType = (int)Constants.Types.AccountTypes.Teacher;
                var disabled = Constants.Status.Disabled;
                var approved = Constants.Status.Approved;
                var hidden = Constants.Status.Hidden;
                tutors = conn.Accounts.Where(t => t.AccountType_ID == teacherType && (t.Approved == approved && t.Disabled != disabled))
                    .ToList();

                if (accountId.HasValue)
                    lessons = rs.Where(l => l.LessonParticipants.All(p => p.Account_ID != accountId) && l.Hidden != hidden).ToList();
                else
                    lessons = rs.Where(l => l.Hidden != hidden)
                        .ToList();

                foreach (var lesson in lessons)
                {
                    foreach (var tutor in tutors)
                    {
                        if (lesson.Account_ID == tutor.ID)
                        {
                            lesson.BookingDate = logic.Rules.Timezone.ConvertFromUTC(lesson.BookingDate);

                            this.classList.Add(new virtRoom
                            {
                                lesson = lesson,
                                tutor = conn.Accounts.FirstOrDefault(t => t.ID == lesson.Account_ID)
                            });

                            break;
                        }
                    }
                }
            }
        }

        public void LoadByTeacher(int teacherId)
        {
            using (var conn = new dbEntities())
            {
                this.tutor = conn.Accounts.FirstOrDefault(a => a.ID == teacherId) ?? new Account();
                this.lessons = conn.Lessons
                            .Where(l => l.Account_ID == teacherId && l.Hidden != true &&
                                    (l.ClassSize == 0 || l.ClassSize > l.LessonParticipants.Count))
                            .ToList();
            }
        }
    }
}
