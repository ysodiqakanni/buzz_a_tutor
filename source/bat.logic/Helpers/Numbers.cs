using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Helpers
{
    public class Numbers
    {
        public static bool IsNumeric(string val)
        {
            if (string.IsNullOrEmpty(val))
                return false;

            int n;
            return int.TryParse(val, out n);
        }

        public static int? ToInt(string val)
        {
            if (string.IsNullOrEmpty(val))
                return null;

            int n;
            if (int.TryParse(val, out n))
                return n;
            else
                return null;
        }
    }
}
