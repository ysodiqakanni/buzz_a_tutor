using bat.data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace bat.logic.ViewModels.Admin
{
    public class Edit
    {
        public string subject { get; set; }
        public SubjectDescription subjectDescription { get; set; }
        public List<SubjectExamPaper> ExamPapers { get; set; }

        public Edit()
        {
            subjectDescription = new SubjectDescription();
            ExamPapers = new List<SubjectExamPaper>();
        }
    }
}
