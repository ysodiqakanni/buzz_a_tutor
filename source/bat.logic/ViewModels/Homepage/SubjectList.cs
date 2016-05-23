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
            var subjectSpace = subject.Replace("_", " ");

            this.subject = subjectSpace;

            using (var conn = new dbEntities())
            {
                this.lessons = conn.Lessons.Where(l => l.Subject == subjectSpace).ToList();
            }
        }
    }
}
