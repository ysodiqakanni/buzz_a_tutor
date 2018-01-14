using bat.data;
using bat.logic.Exceptions;
using EntityFramework.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using OpenTokSDK;

namespace bat.logic.Services
{
    public class Lessons : _ServiceClassBaseMarker
    {
        public ViewModels.Lessons.View LoadView(int id, int loggedInUserId)
        {
            var ret = new ViewModels.Lessons.View()
            {
                account = Helpers.UserAccountInfo.GetAccount(loggedInUserId).account
            };

            using (var conn = new dbEntities())
            {
                ret.lesson = conn.Lessons.FirstOrDefault(l => l.ID == id);
                if (ret.lesson == null) throw new Exception("Lesson does not exist.");

                ret.host = ret.lesson.Account;

                // timezone out for displaying
                ret.lesson.BookingDate = Rules.Timezone.ConvertFromUTC(ret.lesson.BookingDate);

                foreach (var attachment in ret.lesson.LessonAttachments.Select(a => new { a.ID, a.Title }).ToList())
                {
                    ret.attachments.Add(new Models.Lessons.Attachment()
                    {
                        ID = attachment.ID,
                        Title = attachment.Title
                    });
                }

                ret.lessonResources = conn.LessonResources.Where(r => r.Lession_ID == id)
                        .ToList();

                foreach (var participant in ret.lesson.LessonParticipants.ToList())
                {
                    var other = conn.Accounts.FirstOrDefault(a => a.ID == participant.Account_ID);
                    if (other != null)
                        ret.others.Add(other);
                }
            }

            return ret;
        }
        
        public ViewModels.Lessons.View GenerateTokBoxToken(ViewModels.Lessons.View viewModel, int loggedInUserId, string loggedInUserEmail)
        {
            var opentok = new OpenTok(Constants.TokBox.ApiKey, Constants.TokBox.ApiSecret);
            var connectionMetadata = "email=" + loggedInUserEmail + ";accid=" + loggedInUserId;
            if (viewModel.lesson.TokBoxSessionId != null)
            {
                viewModel.token = opentok.GenerateToken(viewModel.lesson.TokBoxSessionId, Role.PUBLISHER, 0, connectionMetadata);
            }
            else
            {
                using (var conn = new dbEntities())
                {
                    var session = opentok.CreateSession("", MediaMode.ROUTED, ArchiveMode.MANUAL);
                    var currentLession = conn.Lessons.FirstOrDefault(l => l.ID == viewModel.lesson.ID);
                    if (currentLession != null)
                    {
                        currentLession.TokBoxSessionId = session.Id;
                        conn.SaveChanges();
                        viewModel.lesson = currentLession;
                    }
                    if (viewModel.lesson.TokBoxSessionId != null)
                    {
                        viewModel.token = opentok.GenerateToken(viewModel.lesson.TokBoxSessionId, Role.PUBLISHER, 0, connectionMetadata);
                    }
                }
            }

            return viewModel;
        }

        public void UploadImage(int lessonId, string title, HttpPostedFileBase data, int loggedInUserId)
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
                    Account_ID = loggedInUserId,
                    Title = title,
                    Data = Convert.ToBase64String(thePictureAsBytes)
                });
                conn.SaveChanges();
            }
        }

        public List<ApiViewModels.Lessons.Upload.lessonAttachment> UploadEncodedImage(int lessonId, int accId, string title, string data)
        {
            var lessonAttachments = new List<ApiViewModels.Lessons.Upload.lessonAttachment>();
            using (var conn = new dbEntities())
            {
                conn.LessonAttachments.Add(new LessonAttachment()
                {
                    Lesson_ID = lessonId,
                    Account_ID = accId,
                    Title = title,
                    Data = data
                });
                conn.SaveChanges();

                var attachments = conn.LessonAttachments
                    .Where(l => l.Lesson_ID == lessonId)
                    .ToList();

                foreach (var attachment in attachments)
                {
                    lessonAttachments.Add(new ApiViewModels.Lessons.Upload.lessonAttachment()
                    {
                        id = attachment.ID,
                        title = attachment.Title
                    });
                }
                return lessonAttachments;
            }
        }

        //public bool CurrentlyAZoomUser
        //{
        //    get
        //    {
        //        try
        //        {
        //            if (string.IsNullOrEmpty(this.account.ZoomUserId)) return false;

        //            // will error if not yet activated
        //            var user = ZoomApi.GetUser(this.account.ZoomUserId);
        //            return true;
        //        }
        //        catch (Exception)
        //        {
        //            return false;
        //        }
        //    }
        //}

        //public void CheckZoomUser()
        //{
        //    if (string.IsNullOrEmpty(account.ZoomUserId))
        //        this.account = Rules.ZoomApi.CreateZoomUserAccount(this.account.ID);
        //}

        //public void CreateZoomMeeting()
        //{
        //    if (string.IsNullOrEmpty(this.host.ZoomUserId))
        //        throw new Exception("The lesson host requires a Zoom account");

        //    if (!string.IsNullOrEmpty(this.lesson.ZoomStartUrl) && !string.IsNullOrEmpty(this.lesson.ZoomJoinUrl))
        //        return;

        //    using (var conn = new dbEntities())
        //    {
        //        this.lesson = conn.Lessons.FirstOrDefault(l => l.ID == this.lesson.ID);
        //        if (this.lesson == null) throw new Exception("Lesson does not exist.");

        //        var zoomLesson = ZoomApi.CreateMeeting(this.host.ZoomUserId, this.lesson.Description);
        //        this.lesson.ZoomStartUrl = zoomLesson.start_url;
        //        this.lesson.ZoomJoinUrl = zoomLesson.join_url;

        //        conn.SaveChanges();
        //    }
        //}

        public ViewModels.Lessons.Cancel LoadCancel(int id, int loggedInUserId)
        {
            var ret = new ViewModels.Lessons.Cancel();

            using (var conn = new dbEntities())
            {
                ret.lesson = conn.Lessons.FirstOrDefault(l => l.ID == id);
                if (ret.lesson == null) throw new Exception("Lesson does not exist.");
                if (ret.lesson.Account_ID != loggedInUserId) throw new WrongAccountException();

                ret.host = ret.lesson.Account;

                // timezone out for displaying
                ret.lesson.BookingDate = Rules.Timezone.ConvertFromUTC(ret.lesson.BookingDate);

                ret.lesson.Subject = ret.lesson.Subject ?? "";
            }

            return ret;
        }

        public void SaveCancel(int lessonId, int loggedInUserId, FormCollection frm)
        {
            using (var conn = new dbEntities())
            {
                var lesson = conn.Lessons.FirstOrDefault(l => l.ID == lessonId);
                if (lesson == null) throw new Exception("Lesson does not exist.");
                if (lesson.Account_ID != loggedInUserId) throw new WrongAccountException();

                lesson.CancelledDate = DateTime.UtcNow;
                conn.SaveChanges();

                Rules.EventLogging.EditLesson(loggedInUserId, lesson);
            }
        }

        public ViewModels.Lessons.Edit LoadEdit(int id, int loggedInUserId)
        {
            var ret = new ViewModels.Lessons.Edit();

            using (var conn = new dbEntities())
            {
                ret.lesson = conn.Lessons.FirstOrDefault(l => l.ID == id);
                if (ret.lesson == null) throw new Exception("Lesson does not exist.");
                if (ret.lesson.Account_ID != loggedInUserId) throw new WrongAccountException();

                // timezone out for displaying
                ret.lesson.BookingDate = Rules.Timezone.ConvertFromUTC(ret.lesson.BookingDate);

                ret.lessonResources = conn.LessonResources.Where(r => r.Lession_ID == id)
                                        .ToList();

                ret.host = ret.lesson.Account;

                ret.lesson.Subject = ret.lesson.Subject ?? "";
            }

            return ret;
        }

        public void SaveEdit(int lessonId, int loggedInUserId, FormCollection frm)
        {
            using (var conn = new dbEntities())
            {
                var description = (frm["Description"] ?? "").Trim();
                if (string.IsNullOrEmpty(description)) throw new Exception("Description is required.");

                var detailedDescription = (frm["DetailedDescription"] ?? "").Trim();
                detailedDescription = HttpUtility.UrlEncode(detailedDescription);

                var lesson = conn.Lessons.FirstOrDefault(l => l.ID == lessonId);
                if (lesson == null) throw new Exception("Lesson does not exist.");
                if (lesson.Account_ID != loggedInUserId) throw new WrongAccountException();

                lesson.DurationMins = int.Parse(frm["DurationMins"]);
                lesson.Description = description;
                lesson.DetailedDescription = detailedDescription;
                lesson.ClassSize = int.Parse(frm["ClassSize"]);
                lesson.Subject = (frm["Subject"] ?? "").Trim();
                conn.SaveChanges();

                Rules.EventLogging.EditLesson(loggedInUserId, lesson);
            }
        }


        public ViewModels.Lessons.Join LoadJoin(int id, int loggedInUserId)
        {
            var ret = new ViewModels.Lessons.Join();

            using (var conn = new dbEntities())
            {
                ret.lesson = conn.Lessons.FirstOrDefault(l => l.ID == id);
                if (ret.lesson == null) throw new Exception("Lesson does not exist.");

                ret.Subject = ret.lesson.Subject;

                // timezone out for displaying
                ret.lesson.BookingDate = Rules.Timezone.ConvertFromUTC(ret.lesson.BookingDate);

                ret.teacher = conn.Accounts.FirstOrDefault(t => t.ID == ret.lesson.Account_ID);

                if (ret.lesson.Account_ID == loggedInUserId)
                {
                    ret.CanContinue = false;
                    return ret;
                }

                if (conn.LessonParticipants.Any(l => l.Account_ID == loggedInUserId && l.Lesson_ID == ret.lesson.ID))
                {
                    ret.CanContinue = false;
                    return ret;
                }

                if (ret.lesson.ClassSize <= 0)
                {
                    ret.CanContinue = true;
                    return ret;
                }

                ret.CanContinue = ret.lesson.LessonParticipants.Count() < ret.lesson.ClassSize;
                return ret;
            }
        }

        public void SaveJoin(int id, int loggedInUserId)
        {
            using (var conn = new dbEntities())
            {
                var lesson = conn.Lessons.FirstOrDefault(l => l.ID == id);
                if (lesson == null) throw new Exception("Lesson does not exist.");

                if (lesson.Account_ID == loggedInUserId)
                    return;

                if (conn.LessonParticipants.Any(l => l.Account_ID == loggedInUserId && l.Lesson_ID == lesson.ID))
                    return;

                if (lesson.ClassSize > 0 &&
                    conn.LessonParticipants.Count(l => l.Lesson_ID == lesson.ID) >= lesson.ClassSize)
                    throw new Exception("Class is full.");

                conn.LessonParticipants.Add(new LessonParticipant()
                {
                    Account_ID = loggedInUserId,
                    Lesson_ID = lesson.ID
                });
                conn.SaveChanges();

                var accInfo = logic.Helpers.UserAccountInfo.Get(loggedInUserId);
                Rules.EventLogging.JoinLesson(lesson.Account_ID, lesson, loggedInUserId, accInfo.account.Fname, accInfo.account.Lname, accInfo.account.Email);
            }
        }

        public ViewModels.Lessons.Leave LoadLeave(int id, int loggedInUserId)
        {
            var ret = new ViewModels.Lessons.Leave();

            using (var conn = new dbEntities())
            {
                ret.lesson = conn.Lessons.FirstOrDefault(l => l.ID == id);
                if (ret.lesson == null) throw new Exception("Lesson does not exist.");

                // timezone out for displaying
                ret.lesson.BookingDate = Rules.Timezone.ConvertFromUTC(ret.lesson.BookingDate);

                ret.CanContinue = conn.LessonParticipants.Any(l => l.Account_ID == loggedInUserId && l.Lesson_ID == ret.lesson.ID);
            }

            return ret;
        }

        public void SaveLeave(int lessonid, int studentid)
        {
            using (var conn = new dbEntities())
            {
                var participants = conn.LessonParticipants.Where(i => i.Lesson_ID == lessonid && i.Account_ID == studentid);
                conn.LessonParticipants.RemoveRange(participants);
                conn.SaveChanges();
            }
        }



        public ViewModels.Lessons.New LoadNew(string date)
        {
            var ret = new ViewModels.Lessons.New();

            if (string.IsNullOrEmpty(date)) return ret;

            var bits = date.Split('-');
            if (bits.Length < 3) return ret;

            ret.lesson.BookingDate = new DateTime(
                Convert.ToInt32(bits[0]), Convert.ToInt32(bits[1]), Convert.ToInt32(bits[2]), 9, 0, 0);

            return ret;
        }

        public ViewModels.Lessons.New SaveNew(FormCollection frm, int loggedInUserId)
        {
            var ret = new ViewModels.Lessons.New();

            // expect dd mm yyyy hh:mm tt
            var bkdt = (frm["BookingDate"] ?? "").Trim();
            if (string.IsNullOrEmpty(bkdt)) throw new Exception("Booking date is required.");

            using (var conn = new dbEntities())
            {
                var description = (frm["Description"] ?? "").Trim();
                if (string.IsNullOrEmpty(description)) throw new Exception("Description is required.");

                if (Rules.Timezone.ConvertToUTC(Convert.ToDateTime(bkdt)) < DateTime.UtcNow)
                    throw new Exception("Booking date not valid.");

                var detailedDescription = (frm["DetailedDescription"] ?? "").Trim();
                detailedDescription = HttpUtility.UrlEncode(detailedDescription);

                // TokBox disabled for now
                //// note, relayed can't be archived (saved)
                //// when saving or archiving video, must be routed not relayed
                var tok = new OpenTok(Constants.TokBox.ApiKey, Constants.TokBox.ApiSecret);
                var session = tok.CreateSession("", MediaMode.RELAYED, ArchiveMode.MANUAL);
                var tokSessionId = session.Id;
                //string tokSessionId = null;

                ret.lesson = new Lesson()
                {
                    Account_ID = loggedInUserId,
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
                conn.Lessons.Add(ret.lesson);
                conn.SaveChanges();

                Rules.EventLogging.CreateLesson(loggedInUserId, ret.lesson);

                return ret;
            }
        }


        public ViewModels.Lessons.Reschedule LoadReschedule(int id, int loggedInUserId)
        {
            var ret = new ViewModels.Lessons.Reschedule();

            using (var conn = new dbEntities())
            {
                ret.lesson = conn.Lessons.FirstOrDefault(l => l.ID == id);
                if (ret.lesson == null) throw new Exception("Lesson does not exist.");
                if (ret.lesson.Account_ID != loggedInUserId) throw new WrongAccountException();

                ret.host = ret.lesson.Account;

                // timezone out for displaying
                ret.lesson.BookingDate = Rules.Timezone.ConvertFromUTC(ret.lesson.BookingDate);

                ret.lesson.Subject = ret.lesson.Subject ?? "";
            }

            return ret;
        }

        public ViewModels.Lessons.Reschedule SaveReschedule(int lessonId, int loggedInUserId, FormCollection frm)
        {
            var ret = new ViewModels.Lessons.Reschedule();

            using (var conn = new dbEntities())
            {
                ret.lesson = conn.Lessons.FirstOrDefault(l => l.ID == lessonId);
                if (ret.lesson == null) throw new Exception("Lesson does not exist.");
                if (ret.lesson.Account_ID != loggedInUserId) throw new WrongAccountException();

                // expect dd mm yyyy hh:mm tt
                var bkdt = (frm["BookingDate"] ?? "").Trim();
                if (string.IsNullOrEmpty(bkdt)) throw new Exception("Booking date is required.");

                ret.lesson.BookingDate = Rules.Timezone.ConvertToUTC(Convert.ToDateTime(bkdt));
                conn.SaveChanges();

                Rules.EventLogging.EditLesson(loggedInUserId, ret.lesson);
            }

            return ret;
        }

        public string GetImageStream(int attachmentId)
        {
            using (var conn = new dbEntities())
            {
                var attachment = conn.LessonAttachments.FirstOrDefault(a => a.ID == attachmentId);
                if (attachment == null)
                    throw new Exception("Attachment not found.");

                return attachment.Data;
            }
        }

        public ApiViewModels.Lessons.GetAttachment.blackboardImage GetBlackboardImages(int lessonId)
        {
            using (var conn = new dbEntities())
            {
                var bbImageList = new ApiViewModels.Lessons.GetAttachment.blackboardImage();
                try
                {
                    //fetch last attached resorce with lesson
                    var resources = conn.LessonResources.Where(a => a.Lession_ID == lessonId && a.Type_ID == Constants.Types.Image).OrderByDescending(rid => rid.ID).FirstOrDefault();
                    if (resources == null)
                        throw new Exception("Attachment not found.");
                    bbImageList.id = Convert.ToString(resources.ID);
                    bbImageList.title = resources.Original_Name;
                }
                catch (Exception)
                {
                    //throw;
                }

                return bbImageList;
            }
        }
    }
}
