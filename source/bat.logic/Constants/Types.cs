﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Constants
{
    public class Types
    {
        public enum AccountTypes
        {
            Student = 1,
            Teacher = 2
        }

        public enum FileTypes
        {
            Default = 1,
            Image = 2,
        }

        public static int Student = (int)AccountTypes.Student;
        public static int Teacher = (int)AccountTypes.Teacher;
        public static int Default = (int)FileTypes.Default;
        public static int Image = (int)FileTypes.Image;
    }
}
