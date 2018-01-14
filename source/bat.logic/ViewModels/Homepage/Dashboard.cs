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
    public class Dashboard
    {
        public Models.AccountInfo AccInfo { get; set; }
        public List<Lesson> lessons { get; set; }
        public List<Lesson> today_lessons { get; set; }
        public class classroom
        {
            public Lesson lesson { get; set; }
            public List<LessonParticipant> participants { get; set; } 
        }
        public List<classroom> classrooms { get; set; }
    }
}
