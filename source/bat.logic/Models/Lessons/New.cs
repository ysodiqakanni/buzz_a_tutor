using bat.data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using OpenTokSDK;

namespace bat.logic.Models.Lessons
{
    public class New : Master
    {
        public Lesson lesson { get; set; }

        public New()
        {
            this.lesson = new Lesson()
            {
                BookingDate = Shearnie.Net.OzTime.GetNowAEST(),
                DurationMins = 15,
                ClassSize = 0
            };
        }

        public bool IsTeacher =>
            this.account.AccountType_ID == (int)bat.logic.Constants.Types.AccountTypes.Teacher;

        public void Save(FormCollection frm)
        {
            // expect dd mm yyyy hh:mm tt
            var bkdt = (frm["BookingDate"] ?? "").Trim();
            if (string.IsNullOrEmpty(bkdt)) throw new Exception("Booking date is required.");

            using (var conn = new dbEntities())
            {
                var opentok = new OpenTok(Constants.TokBox.ApiKey, Constants.TokBox.ApiSecret);

                this.lesson = new Lesson()
                {
                    Account_ID = this.account.ID,
                    BookingDate = Convert.ToDateTime(bkdt, new CultureInfo("en-AU")),
                    DurationMins = int.Parse(frm["DurationMins"]),
                    Description = (frm["Description"] ?? "").Trim(),
                    ClassSize = int.Parse(frm["ClassSize"]),

                    // note, relayed can't be archived (saved)
                    // when saving or archiving video, must be routed not relayed
                    TokBoxSessionId = opentok.CreateSession("", MediaMode.RELAYED, ArchiveMode.MANUAL).Id
                };
                conn.Lessons.Add(this.lesson);
                conn.SaveChanges();
            }
        }
    }
}
