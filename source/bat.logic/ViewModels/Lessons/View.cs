using bat.data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using bat.logic.Constants;
using bat.logic.Models.Lessons;
using bat.logic.Rules;
using OpenTokSDK;
using bat.logic.Exceptions;
using bat.logic.Models;

namespace bat.logic.ViewModels.Lessons
{
    public class View : Partials.IPartialWhiteboard
    {
        public string token { get; set; }

        public Account host { get; set; }
        public List<Account> others { get; set; }

        public Lesson lesson { get; set; }
        public List<Attachment> attachments { get; set; }
        public List<LessonResource> lessonResources { get; set; }

        public Account account { get; set; }
        public AccountInfo AccInfo => new AccountInfo()
        {
            account = account
        };

        public View()
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
            this.attachments = new List<Attachment>();
            this.host = new Account();
            this.others = new List<Account>();
        }
        
        public bool LessonReady
            => !string.IsNullOrEmpty(this.lesson.ZoomStartUrl) && !string.IsNullOrEmpty(this.lesson.ZoomJoinUrl);
    }
}
