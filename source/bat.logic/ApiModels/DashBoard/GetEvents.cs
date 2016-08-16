using bat.data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using bat.logic.Rules;

namespace bat.logic.ApiModels.DashBoard
{
    public class GetEvents
    {
        public class calEvent
        {
            public int id { get; set; }
            public string title { get; set; }
            public string start { get; set; }
            public string color { get; set; }

        }

        public List<calEvent> CalEvents { get; set; }

        public static string GetLessons(int userID)
        {
            var json = new JavaScriptSerializer();

            var calEvents = new List<calEvent>();

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
                            .ToList();

                        foreach (var l in lessons)
                        {
                            calEvents.Add(new calEvent
                            {
                                id = l.ID,
                                title = l.Subject,
                                start = Timezone.ConvertFromUTC(l.BookingDate).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"),
                                color = "#3a87ad"

                            });
                        }
                        if (lessons == null)
                            throw new Exception("Lessons not found.");

                        break;

                    case 2: // Teacher
                        var ownLessons = conn.Lessons.Where(p => p.Account_ID == userID)
                            .ToList();

                        foreach (var l in ownLessons)
                        {
                            calEvents.Add(new calEvent
                            {
                                id = l.ID,
                                title = l.Subject,
                                start = Timezone.ConvertFromUTC(l.BookingDate).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"),
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
