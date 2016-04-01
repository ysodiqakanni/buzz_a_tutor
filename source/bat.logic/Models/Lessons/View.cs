using bat.data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace bat.logic.Models.Lessons
{
    public class View : Master, Partials.IPartialWhiteboard
    {
        public string session { get; set; }
        public string token { get; set; }

        public Account host { get; set; }
        public List<Account> others { get; set; }

        public Lesson lesson { get; set; }
        public List<bat.data.LessonAttachment> attachments { get; set; }

        public View()
        {
            this.lesson = new Lesson()
            {
                BookingDate = Shearnie.Net.OzTime.GetNowAEST(),
                DurationMins = 15,
                ClassSize = 1
            };
            this.attachments = new List<LessonAttachment>();
            this.session = "2_MX40NTQ5NjY1Mn5-MTQ1ODI1NjE1Nzk0OH5OTnRSTUR5c0FZMnpSYkFob1doR2xNT3h-UH4";
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

                this.attachments = this.lesson.LessonAttachments.ToList();

                this.host = this.lesson.Account;
                foreach (var participant in this.lesson.LessonParticipants.ToList())
                {
                    var other = conn.Accounts.FirstOrDefault(a => a.ID == participant.Account_ID);
                    if (other != null)
                        this.others.Add(other);
                }
            }

            if (this.host.Email == "alex")
                this.token = "T1==cGFydG5lcl9pZD00NTQ5NjY1MiZzaWc9ODgzMzExYmRlM2NiMmRmMDNkZTdjYjU2NGZiYWMxZDkwZGJkZjM5YTpyb2xlPXB1Ymxpc2hlciZzZXNzaW9uX2lkPTJfTVg0ME5UUTVOalkxTW41LU1UUTFPREkxTmpFMU56azBPSDVPVG5SU1RVUjVjMEZaTW5wU1lrRm9iMWRvUjJ4TlQzaC1VSDQmY3JlYXRlX3RpbWU9MTQ1ODI1NjE4MCZub25jZT0wLjM1NjU1ODUwNzMyMjAyNzImZXhwaXJlX3RpbWU9MTQ2MDg0NzkzNiZjb25uZWN0aW9uX2RhdGE9";
            else if (this.host.Email == "steve")
                this.token = "T1==cGFydG5lcl9pZD00NTQ5NjY1MiZzaWc9MGM2NTZjOTBlN2JhZWI5OTVkOTEwNDk3ZWM2ODNlNWY1YTk5ZDhmNzpyb2xlPXB1Ymxpc2hlciZzZXNzaW9uX2lkPTJfTVg0ME5UUTVOalkxTW41LU1UUTFPREkxTmpFMU56azBPSDVPVG5SU1RVUjVjMEZaTW5wU1lrRm9iMWRvUjJ4TlQzaC1VSDQmY3JlYXRlX3RpbWU9MTQ1ODI1NjIxNiZub25jZT0wLjQzMjU0MTk3Njc3OTEzODEmZXhwaXJlX3RpbWU9MTQ2MDg0NzkzNiZjb25uZWN0aW9uX2RhdGE9";
            else
                this.token = "T1==cGFydG5lcl9pZD00NTQ5NjY1MiZzaWc9ZDg1MGZiNWNkZWMxMjBhYzE2NWJjNWNhYzIwYzk3YTYxNThiZTRjMTpyb2xlPXB1Ymxpc2hlciZzZXNzaW9uX2lkPTJfTVg0ME5UUTVOalkxTW41LU1UUTFPREkxTmpFMU56azBPSDVPVG5SU1RVUjVjMEZaTW5wU1lrRm9iMWRvUjJ4TlQzaC1VSDQmY3JlYXRlX3RpbWU9MTQ1ODI1NjIzMyZub25jZT0wLjY2MDc2MzUwMjgzODQ5OTImZXhwaXJlX3RpbWU9MTQ2MDg0NzkzNiZjb25uZWN0aW9uX2RhdGE9";
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

        public void UploadImage(int lessonId, string title, HttpPostedFileBase data)
        {
            byte[] thePictureAsBytes = new byte[data.ContentLength];
            using (BinaryReader theReader = new BinaryReader(data.InputStream))
            {
                thePictureAsBytes = theReader.ReadBytes(data.ContentLength);
            }

            using (var conn = new dbEntities())
            {
                conn.LessonAttachments.Add(new LessonAttachment()
                {
                    Lesson_ID = lessonId,
                    Account_ID = this.account.ID,
                    Title = title,
                    Data = Convert.ToBase64String(thePictureAsBytes)
                });
                conn.SaveChanges();
            }
        }
    }
}
