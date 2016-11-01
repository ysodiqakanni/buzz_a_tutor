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

namespace bat.logic.ViewModels.Lessons
{
    public class Reschedule : Master
    {
        public string token { get; set; }

        public Account host { get; set; }

        public Lesson lesson { get; set; }

        public LessonResource lessonResource { get; set; }

        public List<LessonResource> lessonResources { get; set; }

        public Reschedule()
        {
            this.lesson = new Lesson()
            {
                BookingDate = DateTime.UtcNow,
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
                this.lesson = conn.Lessons.FirstOrDefault(l => l.ID == lessonId);
                if (this.lesson == null) throw new Exception("Lesson does not exist.");

                // expect dd mm yyyy hh:mm tt
                var bkdt = (frm["BookingDate"] ?? "").Trim();
                if (string.IsNullOrEmpty(bkdt)) throw new Exception("Booking date is required.");

                this.lesson.BookingDate = Rules.Timezone.ConvertToUTC(Convert.ToDateTime(bkdt));
                conn.SaveChanges();

                Rules.EventLogging.EditLesson(this.account, this.lesson);
            }
        }
    }
}