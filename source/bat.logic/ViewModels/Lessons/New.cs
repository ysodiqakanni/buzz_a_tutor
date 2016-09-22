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
    public class New : Master
    {
        public Lesson lesson { get; set; }

        public New()
        {
            this.lesson = new Lesson()
            {
                BookingDate = DateTime.UtcNow,
                DurationMins = 15,
                ClassSize = 0
            };
        }

        public bool IsTeacher =>
            this.account.AccountType_ID == (int)bat.logic.Constants.Types.AccountTypes.Teacher &&
            (this.account.Approved ?? false) == Constants.Status.Approved &&
            (this.account.Disabled ?? false) == Constants.Status.Enabled;

        public void Save(FormCollection frm)
        {
            // expect dd mm yyyy hh:mm tt
            var bkdt = (frm["BookingDate"] ?? "").Trim();
            if (string.IsNullOrEmpty(bkdt)) throw new Exception("Booking date is required.");

            using (var conn = new dbEntities())
            {
                var description = (frm["Description"] ?? "").Trim();
                if (string.IsNullOrEmpty(description)) throw new Exception("Description is required.");

                var detailedDescription = (frm["DetailedDescription"] ?? "").Trim();
                detailedDescription = HttpUtility.UrlEncode(detailedDescription);

                // TokBox disabled for now
                //// note, relayed can't be archived (saved)
                //// when saving or archiving video, must be routed not relayed
                //var tok = new OpenTok(Constants.TokBox.ApiKey, Constants.TokBox.ApiSecret);
                //var session = tok.CreateSession("", MediaMode.RELAYED, ArchiveMode.MANUAL);
                //var tokSessionId = session.Id;
                string tokSessionId = null;

                this.lesson = new Lesson()
                {
                    Account_ID = this.account.ID,
                    BookingDate = Rules.Timezone.ConvertToUTC(Convert.ToDateTime(bkdt)),
                    DurationMins = int.Parse(frm["DurationMins"]),
                    Description = description,
                    DetailedDescription = detailedDescription,
                    ClassSize = int.Parse(frm["ClassSize"]),
                    Subject = (frm["Subject"] ?? "").Trim(),

                    TokBoxSessionId = tokSessionId,
                    ZoomStartUrl = "",
                    ZoomJoinUrl = ""
                };
                conn.Lessons.Add(this.lesson);
                conn.SaveChanges();
            }
        }
    }
}
