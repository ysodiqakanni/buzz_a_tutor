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
    public class Edit : Master
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

        public void Load(int id)
        {
            using (var conn = new dbEntities())
            {
                this.lesson = conn.Lessons.FirstOrDefault(l => l.ID == id);
                if (this.lesson == null) throw new Exception("Lesson does not exist.");
                if (this.lesson.Account_ID != this.account.ID) throw new WrongAccountException();

                // timezone out for displaying
                this.lesson.BookingDate = Rules.Timezone.ConvertFromUTC(this.lesson.BookingDate);

                this.lessonResources = conn.LessonResources.Where(r => r.Lession_ID == id)
                                        .ToList();

                this.host = this.lesson.Account;

                this.lesson.Subject = this.lesson.Subject ?? "";
            }
        }

        public bool IsTeacher =>
            this.account.AccountType_ID == (int)bat.logic.Constants.Types.AccountTypes.Teacher;

        public void Save(int lessonId, FormCollection frm)
        {
            using (var conn = new dbEntities())
            {
                var description = (frm["Description"] ?? "").Trim();
                if (string.IsNullOrEmpty(description)) throw new Exception("Description is required.");

                var detailedDescription = (frm["DetailedDescription"] ?? "").Trim();
                detailedDescription = HttpUtility.UrlEncode(detailedDescription);

                this.lesson = conn.Lessons.FirstOrDefault(l => l.ID == lessonId);
                if (this.lesson == null) throw new Exception("Lesson does not exist.");
                if (this.lesson.Account_ID != this.account.ID) throw new WrongAccountException();

                this.host = this.lesson.Account;

                this.lesson.DurationMins = int.Parse(frm["DurationMins"]);
                this.lesson.Description = description;
                this.lesson.DetailedDescription = detailedDescription;
                this.lesson.ClassSize = int.Parse(frm["ClassSize"]);
                this.lesson.Subject = (frm["Subject"] ?? "").Trim();
                conn.SaveChanges();

                Rules.EventLogging.EditLesson(this.account, this.lesson);
            }
        }
    }
}
