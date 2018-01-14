using bat.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.ViewModels.Admin
{
    public class Lessons
    {
        public List<bat.data.Lesson> lessons { get; set; }
        public Account tutor { get; set; }
    }
}
