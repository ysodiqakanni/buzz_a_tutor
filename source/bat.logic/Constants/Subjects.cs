using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.logic.Models.Subjects;

namespace bat.logic.Constants
{
    public class Subjects
    {
        public static Department Maths => new Department()
        {
            Name = "Maths",
            Subjects = new List<string>()
            {
                "GCSE Maths",
                "A Level Maths"
            }
        };

        public static Department English => new Department()
        {
            Name = "English",
            Subjects = new List<string>()
            {
                "GCSE English",
                "A Level English"
            }
        };

        public static Department Science => new Department()
        {
            Name = "Science",
            Subjects = new List<string>()
            {
                "Biology",
                "Chemistry",
                "Physics"
            }
        };

        public static Department Languages => new Department()
        {
            Name = "Languages",
            Subjects = new List<string>()
            {
                "German",
                "French",
                "Japanese"
            }
        };

        public static Department Other => new Department()
        {
            Name = "Other",
            Subjects = new List<string>()
            {
                "History",
                "Geography",
                "Computing"
            }
        };

        public static List<Department> Departments => new List<Department>()
        {
            Maths, English, Science, Languages, Other
        };
    }
}
