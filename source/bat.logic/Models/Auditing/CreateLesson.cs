using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Models.Auditing
{
    public class CreateLesson
    {
        public int ID { get; set; }
        public int Account_ID { get; set; }
        public string Description { get; set; }
        public static string TypeToString()
        {
            return "Create Lesson";
        }
    }
}
