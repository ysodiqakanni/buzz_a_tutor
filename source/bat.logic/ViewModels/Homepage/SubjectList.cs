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

        public void Load(string subject)
        {
            this.subject = subject.Replace("_", " ");

            using (var conn = new dbEntities())
            {
                this.lessons = conn.Lessons
                                        .Where(l => l.Subject == this.subject && 
                                              (l.ClassSize == 0 || l.ClassSize > l.LessonParticipants.Count)).ToList();
            }
        }
    }
}
