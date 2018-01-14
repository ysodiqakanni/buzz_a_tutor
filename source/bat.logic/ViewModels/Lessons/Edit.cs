using bat.data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using bat.logic.Constants;
using bat.logic.Models.Lessons;
using bat.logic.Rules;
using OpenTokSDK;
using bat.logic.Exceptions;

namespace bat.logic.ViewModels.Lessons
{
    public class Edit
    {
        public string token { get; set; }
        public Account host { get; set; }
        public Lesson lesson { get; set; }
        public LessonResource lessonResource { get; set; }
        public List<LessonResource> lessonResources { get; set; }

        public Edit()
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
            this.host = new Account();

            this.lessonResource = new LessonResource();
        }
    }
}
