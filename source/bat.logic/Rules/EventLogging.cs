using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using bat.logic.Models.Auditing;
using Newtonsoft.Json;

namespace bat.logic.Rules
{
    class EventLogging
    {
        public static void Login(Account user)
        {
            using (var conn = new dbEntities())
            {
                conn.Configuration.AutoDetectChangesEnabled = false;
                conn.Configuration.ValidateOnSaveEnabled = false;

                conn.EventLogs.Add(new EventLog()
                {
                    Account_ID = user.ID,
                    Type = Models.Auditing.AccountLogin.TypeToString(),
                    Data = JsonConvert.SerializeObject(
                    new Models.Auditing.AccountLogin()
                    {
                        ID = user.ID,
                        Email = user.Email,
                        Fname = user.Fname,
                        Lname = user.Lname
                    }),
                    EventDate = DateTime.UtcNow,
                    IPAddress = Shearnie.Net.Web.ServerInfo.GetIPAddress
                });
                conn.SaveChanges();
            }
        }

        public static void Register(Account user)
        {
            using (var conn = new dbEntities())
            {
                conn.Configuration.AutoDetectChangesEnabled = false;
                conn.Configuration.ValidateOnSaveEnabled = false;

                conn.EventLogs.Add(new EventLog()
                {
                    Account_ID = user.ID,
                    Type = Models.Auditing.AccountRegistration.TypeToString(),
                    Data = JsonConvert.SerializeObject(
                        new Models.Auditing.AccountRegistration()
                        {
                            ID = user.ID,
                            AccountType_ID = user.AccountType_ID,
                            AccountType = ((logic.Constants.Types.AccountTypes)user.AccountType_ID).ToString(),
                            Email = user.Email,
                            Fname = user.Fname,
                            Lname = user.Lname
                        }),
                    EventDate = DateTime.UtcNow,
                    IPAddress = Shearnie.Net.Web.ServerInfo.GetIPAddress
                });
                conn.SaveChanges();
            }
        }

        public static void CreateLesson(int teacherId, bat.data.Lesson lesson)
        {
            using (var conn = new dbEntities())
            {
                conn.Configuration.AutoDetectChangesEnabled = false;
                conn.Configuration.ValidateOnSaveEnabled = false;

                conn.EventLogs.Add(new EventLog()
                {
                    Account_ID = teacherId,
                    Type = Models.Auditing.CreateLesson.TypeToString(),
                    Data = JsonConvert.SerializeObject(
                        new Models.Auditing.CreateLesson()
                        {
                            ID = lesson.ID,
                            Lesson = new Models.Auditing.Lesson()
                            {
                                ID = lesson.ID,
                                Teacher_Account_ID = teacherId,
                                BookingDate = lesson.BookingDate,
                                DurationMins = lesson.DurationMins,
                                Description = lesson.Description,
                                ClassSize = lesson.ClassSize,
                                TokBoxSessionId = lesson.TokBoxSessionId,
                                ZoomStartUrl = lesson.ZoomStartUrl,
                                ZoomJoinUrl = lesson.ZoomJoinUrl,
                                Subject = lesson.Subject,
                                DetailedDescription = lesson.DetailedDescription
                            }
                        }),
                    EventDate = DateTime.UtcNow,
                    IPAddress = Shearnie.Net.Web.ServerInfo.GetIPAddress
                });
                conn.SaveChanges();
            }
        }

        public static void JoinLesson(
            int teacherId, bat.data.Lesson lesson, 
            int studentId, string studentFname, string studentLname, string studentEmail)
        {
            using (var conn = new dbEntities())
            {
                conn.Configuration.AutoDetectChangesEnabled = false;
                conn.Configuration.ValidateOnSaveEnabled = false;

                conn.EventLogs.Add(new EventLog()
                {
                    Account_ID = studentId,
                    Type = Models.Auditing.JoinLesson.TypeToString(),
                    Data = JsonConvert.SerializeObject(
                        new Models.Auditing.JoinLesson()
                        {
                            ID = lesson.ID,
                            Lesson = new Models.Auditing.Lesson()
                            {
                                ID = lesson.ID,
                                Teacher_Account_ID = teacherId,
                                BookingDate = lesson.BookingDate,
                                DurationMins = lesson.DurationMins,
                                Description = lesson.Description,
                                ClassSize = lesson.ClassSize,
                                TokBoxSessionId = lesson.TokBoxSessionId,
                                ZoomStartUrl = lesson.ZoomStartUrl,
                                ZoomJoinUrl = lesson.ZoomJoinUrl,
                                Subject = lesson.Subject,
                                DetailedDescription = lesson.DetailedDescription
                            },
                            Participant_Account_ID = studentId,
                            Participant_Fname = studentFname,
                            Participant_Lname = studentLname,
                            Participant_Email = studentEmail
                        }),
                    EventDate = DateTime.UtcNow,
                    IPAddress = Shearnie.Net.Web.ServerInfo.GetIPAddress
                });
                conn.SaveChanges();
            }
        }
        public static void EditLesson(int teacherId, bat.data.Lesson lesson)
        {
            using (var conn = new dbEntities())
            {
                conn.Configuration.AutoDetectChangesEnabled = false;
                conn.Configuration.ValidateOnSaveEnabled = false;

                conn.EventLogs.Add(new EventLog()
                {
                    Account_ID = teacherId,
                    Type = Models.Auditing.EditLesson.TypeToString(),
                    Data = JsonConvert.SerializeObject(
                        new Models.Auditing.EditLesson()
                        {
                            ID = lesson.ID,
                            Lesson = new Models.Auditing.Lesson()
                            {
                                ID = lesson.ID,
                                Teacher_Account_ID = teacherId,
                                BookingDate = lesson.BookingDate,
                                DurationMins = lesson.DurationMins,
                                Description = lesson.Description,
                                ClassSize = lesson.ClassSize,
                                TokBoxSessionId = lesson.TokBoxSessionId,
                                ZoomStartUrl = lesson.ZoomStartUrl,
                                ZoomJoinUrl = lesson.ZoomJoinUrl,
                                Subject = lesson.Subject,
                                DetailedDescription = lesson.DetailedDescription
                            }
                        }),
                    EventDate = DateTime.UtcNow,
                    IPAddress = Shearnie.Net.Web.ServerInfo.GetIPAddress
                });
                conn.SaveChanges();
            }
        }
    }
}
