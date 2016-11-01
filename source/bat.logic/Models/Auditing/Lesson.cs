using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Models.Auditing
{
    public class Lesson
    {
        public int ID { get; set; }
        public int Teacher_Account_ID { get; set; }
        public System.DateTime BookingDate { get; set; }
        public int DurationMins { get; set; }
        public string Description { get; set; }
        public int ClassSize { get; set; }
        public string TokBoxSessionId { get; set; }
        public string ZoomStartUrl { get; set; }
        public string ZoomJoinUrl { get; set; }
        public string Subject { get; set; }
        public string DetailedDescription { get; set; }
    }
}
