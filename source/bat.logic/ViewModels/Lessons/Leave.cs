using bat.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.ViewModels.Lessons
{
    public class Leave
    {
        public Lesson lesson { get; set; }
        public bool CanContinue { get; set; }
    }
}
