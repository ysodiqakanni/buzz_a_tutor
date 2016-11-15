using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Helpers
{
    public class Strings
    {
        public static string Trunc(string val, int length)
        {
            if (val.Length <= length)
                return val;

            return val.Substring(0, length) + "...";
        }

        public static bool EmailValid(string email)
        {
            var parts = email.Split('@');
            if (parts.Length != 2) return false;

            var domainParts = parts[1].Split('.');
            return domainParts.Length >= 2;
        }
    }
}
