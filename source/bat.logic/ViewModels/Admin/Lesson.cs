using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
namespace bat.logic.ViewModels.Admin
{
    public class Lesson
    {
        public bat.data.Lesson lesson { get; set; }
        public List<LessonParticipant> participants { get; set; }
    }
}
