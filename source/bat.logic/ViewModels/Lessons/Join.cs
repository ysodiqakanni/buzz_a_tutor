using bat.data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace bat.logic.ViewModels.Lessons
{
    public class Join
    {
        public Lesson lesson { get; set; }
        public String Subject { get; set; }
        public Account teacher { get; set; }

        public bool CanContinue { get; set; }

        public Join()
        {
            this.lesson = new Lesson()
            {
                BookingDate = DateTime.UtcNow,
                DurationMins = 15,
                ClassSize = 0,
                TokBoxSessionId = "",
                ZoomStartUrl = "",
                ZoomJoinUrl = ""
            };
        }
    }
}
