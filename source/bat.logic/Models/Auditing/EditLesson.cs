using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Models.Auditing
{
    public class EditLesson
    {
        public int ID { get; set; }
        public Lesson Lesson { get; set; }
        public static string TypeToString()
        {
            return "Edit Lesson";
        }
    }
}
