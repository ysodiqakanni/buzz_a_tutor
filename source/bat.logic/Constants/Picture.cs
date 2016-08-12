using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Constants
{
    class Picture
    {
        public static string Default_Image
        {
            get
            {
#if DEBUG
                return "3166134a-b1c6-4276-88e3-8a56c3564b7e";
#endif
                return "lesson-files";
            }
        }
    }
}

