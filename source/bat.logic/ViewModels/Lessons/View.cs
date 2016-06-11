using bat.data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using bat.logic.Constants;
using bat.logic.Models.Lessons;
using bat.logic.Rules;
using OpenTokSDK;

namespace bat.logic.ViewModels.Lessons
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
                BookingDate = DateTime.UtcNow,
                DurationMins = 15,
                ClassSize = 0,
                TokBoxSessionId = "",
                ZoomStartUrl = "",
                ZoomJoinUrl = ""
            };
            this.attachments = new List<Attachment>();
            this.host = new Account();
            this.others = new List<Account>();

        }

        public bool IsTeacher =>
            this.account.AccountType_ID == (int)bat.logic.Constants.Types.AccountTypes.Teacher;

        public bool WebRTCAvailable
        {
            get
            {
                var browser = Shearnie.Net.Web.ServerInfo.GetBrowser.ToLower();
                if (browser.Contains("firefox"))
                    return true;

                // internet explorer has a plugin available
                if (browser.Contains("internetexplorer"))
                    return true;

                if (browser.Contains("chrome"))
                {
                    // from: https://stackoverflow.com/questions/31870789/check-whether-browser-is-chrome-or-edge
                    if (Shearnie.Net.Web.ServerInfo.GetDevice.IndexOf("Edge") > -1)
                    {
                        return false;
                    }

                    return true;

                }

                if (browser.Contains("chrome"))
                    return false;

                return false;
            }
        }

        public void Load(int id)
        {
            using (var conn = new dbEntities())
            {
                this.lesson = conn.Lessons.FirstOrDefault(l => l.ID == id);
                if (this.lesson == null) throw new Exception("Lesson does not exist.");
                
                // timezone out for displaying
                this.lesson.BookingDate = Rules.Timezone.ConvertFromUTC(this.lesson.BookingDate);

                foreach (var attachment in this.lesson.LessonAttachments.Select(a => new { a.ID, a.Title }).ToList())
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

        public bool CurrentlyAZoomUser
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(this.account.ZoomUserId)) return false;

                    // will error if not yet activated
                    var user = ZoomApi.GetUser(this.account.ZoomUserId);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        public void CreateZoomUser()
        {
            // already registered
            if (!string.IsNullOrEmpty(this.account.ZoomUserId))
                return;

            using (var conn = new dbEntities())
            {
                this.account = conn.Accounts.FirstOrDefault(a => a.ID == this.account.ID);
                if (this.account == null) throw new Exception("Invalid user account.");

                var user = ZoomApi.CreateUser(this.account.Fname, this.account.Lname, this.account.Email, Zoom.UserTypes.Basic);
                this.account.ZoomUserId = user.id;

                conn.SaveChanges();
            }
        }

        public void CreateZoomMeeting()
        {
            if (string.IsNullOrEmpty(this.host.ZoomUserId))
                throw new Exception("The lesson host requires a Zoom account");

            if (!string.IsNullOrEmpty(this.lesson.ZoomStartUrl) && !string.IsNullOrEmpty(this.lesson.ZoomJoinUrl))
                return;

            using (var conn = new dbEntities())
            {
                this.lesson = conn.Lessons.FirstOrDefault(l => l.ID == this.lesson.ID);
                if (this.lesson == null) throw new Exception("Lesson does not exist.");

                var zoomLesson = ZoomApi.CreateMeeting(this.host.ZoomUserId, this.lesson.Description);
                this.lesson.ZoomStartUrl = zoomLesson.start_url;
                this.lesson.ZoomJoinUrl = zoomLesson.join_url;

                conn.SaveChanges();
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
