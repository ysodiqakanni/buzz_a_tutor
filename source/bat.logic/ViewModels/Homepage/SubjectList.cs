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
    public class SubjectList
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
    }
}
