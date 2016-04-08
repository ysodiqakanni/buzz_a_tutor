using bat.data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using OpenTokSDK;

namespace bat.logic.Models.Lessons
{
    public class View : Master, Partials.IPartialWhiteboard
    {
        public string token { get; set; }

        public Account host { get; set; }
        public List<Account> others { get; set; }

        public Lesson lesson { get; set; }
        public List<Attachment> attachments { get; set; }

        public View()
        {
            this.lesson = new Lesson()
            {
                BookingDate = Shearnie.Net.OzTime.GetNowAEST(),
                DurationMins = 15,
                ClassSize = 0,
                TokBoxSessionId = ""
            };
            this.attachments = new List<Attachment>();
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

                foreach(var attachment in this.lesson.LessonAttachments.Select(a => new { a.ID, a.Title }).ToList())
                {
                    this.attachments.Add(new Attachment()
                    {
                        ID = attachment.ID,
                        Title = attachment.Title
                    });
                }

                this.host = this.lesson.Account;
                foreach (var participant in this.lesson.LessonParticipants.ToList())
                {
                    var other = conn.Accounts.FirstOrDefault(a => a.ID == participant.Account_ID);
                    if (other != null)
                        this.others.Add(other);
                }
            }
        }

        public void GenerateTokBoxToken()
        {
            var opentok = new OpenTok(Constants.TokBox.ApiKey, Constants.TokBox.ApiSecret);
            var connectionMetadata = "email=" + this.account.Email + ";accid=" + this.account.ID;
            this.token = opentok.GenerateToken(this.lesson.TokBoxSessionId, Role.PUBLISHER, 0, connectionMetadata);
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
