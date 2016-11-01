using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Models.Auditing
{
    public class JoinLesson
    {
        public int ID { get; set; }
        public Lesson Lesson { get; set; }
        public int Participant_Account_ID { get; set; }
        public string Participant_Fname { get; set; }
        public string Participant_Lname { get; set; }
        public string Participant_Email { get; set; }
        public static string TypeToString()
        {
            return "Join Lesson";
        }
    }
}
