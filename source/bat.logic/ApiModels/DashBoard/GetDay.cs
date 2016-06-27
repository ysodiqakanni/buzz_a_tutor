using bat.data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace bat.logic.ApiModels.DashBoard
{
    public class GetDay
    {
        public class calEvent
        {
            public string id { get; set; }
            public string title { get; set; }
            public string start { get; set; }
            public string end { get; set; }
            public string color { get; set; }
        }
        public List<calEvent> CalEvents { get; set; }

        public static string GDay(int userID, DateTime date)
        {
            var json = new JavaScriptSerializer();

            var calEvents = new List<calEvent>();
            var tomorrow = date.AddDays(1);
            using (var conn = new dbEntities())
            {
                var user = conn.Accounts.FirstOrDefault(a => a.ID == userID);
                if (user == null)
                    throw new Exception("User not found.");

                switch (user.AccountType_ID)
                {
                    case 1: // Student
                        var lessons = conn.LessonParticipants.Where(p => p.Account_ID == userID)
                                        .Select(p => p.Lesson)
                                        .Where(l => l.BookingDate > date && l.BookingDate < tomorrow)
                                        .ToList();
                        foreach (var l in lessons)
                        {
                            var duration = Convert.ToDouble(l.DurationMins);

                            calEvents.Add(new calEvent
                            {
                                id = Convert.ToString(l.ID),
                                title = String.Concat(l.Account.Fname, " ", l.Account.Lname, " - ", l.Description),
                                start = l.BookingDate.ToUniversalTime().ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"),
                                end = l.BookingDate.AddMinutes(duration).ToUniversalTime().ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"),
                                color = "#3a87ad"
                            });

                        }
                        if (lessons == null)
                            throw new Exception("Lessons not found."); 
                        break;

                    case 2: // Teacher
                        var ownLessons = conn.Lessons.Where(p => p.Account_ID == userID)
                            .Where(l => l.BookingDate > date && l.BookingDate < tomorrow)
                            .ToList();

                        foreach (var l in ownLessons)
                        {
                            var duration = Convert.ToDouble(l.DurationMins);

                            calEvents.Add(new calEvent
                            {
                                id = Convert.ToString(l.ID),
                                title = String.Concat(l.Account.Fname, " ", l.Account.Lname, " - ", l.Description),
                                start = l.BookingDate.ToUniversalTime().ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"),
                                end = l.BookingDate.AddMinutes(duration).ToUniversalTime().ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"),
                                color = "#EA8B23"
                            });

                        }
                        if (ownLessons == null)
                            throw new Exception("Lessons not found.");

                        goto case 1;
                }


                return json.Serialize(calEvents);
            }
        }
    }
}
