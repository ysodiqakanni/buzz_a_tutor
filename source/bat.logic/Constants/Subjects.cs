using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Constants
{
    public class Subjects
    {
        public static List<string> Maths => new List<string>()
        {
            "GCSE Maths",
            "A Level Maths"
        };

        public static List<string> English => new List<string>()
        {
            "GCSE English",
            "A Level English"
        };

        public static List<string> Science => new List<string>()
        {
            "Biology" ,
            "Chemistry",
            "Physics"
        };

        public static List<string> Languages => new List<string>()
        {
            "German" ,
            "French",
            "Japanese"
        };
    }
}
