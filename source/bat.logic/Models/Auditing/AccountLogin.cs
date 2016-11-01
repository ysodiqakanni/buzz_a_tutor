using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Models.Auditing
{
    public class AccountLogin
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public static string TypeToString()
        {
            return "Account Login";
        }
    }
}
