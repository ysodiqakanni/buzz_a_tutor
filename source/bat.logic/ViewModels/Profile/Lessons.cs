using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;

namespace bat.logic.ViewModels.Profile
{
    public class Lessons
    {
        public Lesson lesson { get; set; }
        public List<Lesson> lessons { get; set; }
        public List<Lesson> familyLessons { get; set; }
        public List<Lesson> teacherLessons { get; set; }
        public class FamilyRecord
        {
            public Account familyMember { get; set; }
            public List<Lesson> lessons { get; set; }
        }
        
        public List<FamilyRecord> familyRecords { get; set; } 
    }
}