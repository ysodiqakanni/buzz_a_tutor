using bat.data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using OpenTokSDK;

namespace bat.logic.ViewModels.Lessons
{
    public class New
    {
        public Lesson lesson { get; set; }

        public New()
        {
            this.lesson = new Lesson()
            {
                BookingDate = DateTime.UtcNow.AddDays(1),
                DurationMins = 15,
                ClassSize = 0
            };
        }
    }
}
