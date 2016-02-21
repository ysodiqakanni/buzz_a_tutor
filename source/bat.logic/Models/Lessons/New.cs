using bat.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

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
                ClassSize = 1
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
                this.lesson = new Lesson()
                {
                    Account_ID = this.account.ID,
                    BookingDate = new DateTime(
                                    year: Convert.ToInt32(bkdt.Substring(6, 4)),
                                    month: Convert.ToInt32(bkdt.Substring(3, 2)),
                                    day: Convert.ToInt32(bkdt.Substring(0, 2)),
                                    hour: Convert.ToInt32(bkdt.Substring(11, 2)),
                                    minute: Convert.ToInt32(bkdt.Substring(14, 2)),
                                    second: 0),
                    DurationMins = int.Parse(frm["DurationMins"]),
                    Description = (frm["Description"] ?? "").Trim(),
                    ClassSize = int.Parse(frm["ClassSize"])
                };
                conn.Lessons.Add(this.lesson);
                conn.SaveChanges();
            }
        }
    }
}
