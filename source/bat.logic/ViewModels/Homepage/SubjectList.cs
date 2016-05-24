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
    public class SubjectList : Master
    {
        public List<Lesson> lessons { get; set; }
        public String subject { get; set;}

        public class lesson
        {
            public Lesson lessonInfo { get; set; }
            public int openSlots { get; set; }
        }

        public List<lesson> subjectLessons { get; set; }

        public void Load(string subject)
        {
            var subjectSpace = subject.Replace("_", " ");

            this.subjectLessons = new List<lesson>();
            this.subject = subjectSpace;

            using (var conn = new dbEntities())
            {
                var lessons = conn.Lessons
                                        .Where(l => l.Subject == subjectSpace && 
                                              (l.ClassSize == 0 || l.ClassSize > l.LessonParticipants.Count)).ToList();

                foreach (var lesson in lessons)
                {

                    List<LessonParticipant> participants = conn.LessonParticipants
                                        .Where(l => l.Lesson_ID == lesson.ID).ToList();

                    this.subjectLessons.Add(new lesson
                    {
                        lessonInfo = lesson,
                        openSlots = (lesson.ClassSize - participants.Count())
                    });
                }
            }
        }
    }
}
