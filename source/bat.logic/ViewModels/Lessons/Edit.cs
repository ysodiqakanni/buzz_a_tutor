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
    public class Edit : Master
    {
        public string token { get; set; }

        public Account host { get; set; }

        public Lesson lesson { get; set; }

        public LessonResource lessonResource { get; set; }

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
                
                // timezone out for displaying
                this.lesson.BookingDate = Rules.Timezone.ConvertFromUTC(this.lesson.BookingDate);
                
                this.host = this.lesson.Account;
            }
        }

        public bool IsTeacher =>
            this.account.AccountType_ID == (int)bat.logic.Constants.Types.AccountTypes.Teacher;

        public void Save(int lessonId, FormCollection frm, HttpPostedFileBase classResource)
        {
            using (var conn = new dbEntities())
            {
                var description = (frm["Description"] ?? "").Trim();
                if (string.IsNullOrEmpty(description)) throw new Exception("Description is required.");

                var detailedDescription = (frm["DetailedDescription"] ?? "").Trim();
                detailedDescription = HttpUtility.UrlEncode(detailedDescription);

                this.lesson = conn.Lessons.FirstOrDefault(l => l.ID == lessonId);
                if (this.lesson == null) throw new Exception("Lesson does not exist.");

                this.lesson.DurationMins = int.Parse(frm["DurationMins"]);
                this.lesson.Description = description;
                this.lesson.DetailedDescription = detailedDescription;
                this.lesson.ClassSize = int.Parse(frm["ClassSize"]);
                this.lesson.Subject = (frm["Subject"] ?? "").Trim();
                if (classResource != null)
                {
                    this.lessonResource = new LessonResource()
                    {
                        Lession_ID = lessonId,
                        Original_Name = classResource.FileName,
                        Item_Storage_Name = logic.Helpers.AzureStorage.UploadFile.Upload(classResource)
                    };
                    conn.LessonResources.Add(this.lessonResource);
                }
                conn.SaveChanges();
            }
        }
    }
}
