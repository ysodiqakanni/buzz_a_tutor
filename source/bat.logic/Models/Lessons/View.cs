using bat.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace bat.logic.Models.Lessons
{
    public class View : Master
    {
        public Lesson lesson { get; set; }
        public string session { get; set; }
        public string token { get; set; }

        public Account host { get; set; }
        public List<Account> others { get; set; }

        public View()
        {
            this.lesson = new Lesson()
            {
                BookingDate = Shearnie.Net.OzTime.GetNowAEST(),
                DurationMins = 15,
                ClassSize = 1
            };

            this.session = "1_MX40NTQ5NjY1Mn5-MTQ1NjAxMDQ5NjI4MH5MTUNGT3R6RThEVGtxaFJwSTNoRXBpUWd-UH4";
            this.host = new Account();
            this.others = new List<Account>();
        }

        public bool IsTeacher =>
            this.account.AccountType_ID == (int)bat.logic.Constants.Types.AccountTypes.Teacher;

        public void Load(int id)
        {
            using (var conn = new dbEntities())
            {
                this.lesson = conn.Lessons.FirstOrDefault(l => l.ID == id);
                if (this.lesson == null) throw new Exception("Lesson does not exist.");

                this.host = this.lesson.Account;
                foreach (var participant in this.lesson.LessonParticipants.ToList())
                {
                    var other = conn.Accounts.FirstOrDefault(a => a.ID == participant.Account_ID);
                    if (other != null)
                        this.others.Add(other);
                }
            }

            if (this.host.Email == "alex")
                this.token = "T1==cGFydG5lcl9pZD00NTQ5NjY1MiZzaWc9NzZkM2Q1YmM5MWQyZmUzZDcwMzZiZmU4MDQxOTA4MTRmNDk3NmY2Yjpyb2xlPXB1Ymxpc2hlciZzZXNzaW9uX2lkPTFfTVg0ME5UUTVOalkxTW41LU1UUTFOakF4TURRNU5qSTRNSDVNVFVOR1QzUjZSVGhFVkd0eGFGSndTVE5vUlhCcFVXZC1VSDQmY3JlYXRlX3RpbWU9MTQ1NjAxMTkyMyZub25jZT0wLjMyMTY5MTkwNTc1NTYxMTQ2JmV4cGlyZV90aW1lPTE0NTg2MDA5ODcmY29ubmVjdGlvbl9kYXRhPQ==";
            else
                this.token = "T1==cGFydG5lcl9pZD00NTQ5NjY1MiZzaWc9OGE4OTZjZGM1NGQyZTMzMDI1YmI0NzQ4ODZlZDlhM2Y5ZjFjNDEzNDpyb2xlPXB1Ymxpc2hlciZzZXNzaW9uX2lkPTFfTVg0ME5UUTVOalkxTW41LU1UUTFOakF4TURRNU5qSTRNSDVNVFVOR1QzUjZSVGhFVkd0eGFGSndTVE5vUlhCcFVXZC1VSDQmY3JlYXRlX3RpbWU9MTQ1NjAxMTk3OSZub25jZT0wLjg4MzcxMDU4NDg4MDIyMDQmZXhwaXJlX3RpbWU9MTQ1ODYwMDk4NyZjb25uZWN0aW9uX2RhdGE9";
        }

        public void Save(FormCollection frm)
        {
            var lessonId = int.Parse(frm["lessonId"]);

            using (var conn = new dbEntities())
            {
                if (conn.LessonParticipants.Any(l => l.Account_ID == this.account.ID && l.Lesson_ID == lessonId))
                    return;

                conn.LessonParticipants.Add(new LessonParticipant()
                {
                    Account_ID = this.account.ID,
                    Lesson_ID = lessonId
                });
                conn.SaveChanges();
            }
        }
    }
}
