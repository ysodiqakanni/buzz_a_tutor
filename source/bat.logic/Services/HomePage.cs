using bat.data;
using bat.logic.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using bat.logic.Rules;

namespace bat.logic.Services
{
    public class HomePage : _ServiceClassBaseMarker
    {
        public ViewModels.Homepage.Dashboard LoadDashboard(Constants.Types.AccountTypes accountType, int accountId)
        {
            using (var conn = new dbEntities())
            {
                var ret = new ViewModels.Homepage.Dashboard();

                switch (accountType)
                {
                    case Types.AccountTypes.Student:
                        ret.today_lessons = new List<Lesson>();
                        ret.lessons =
                            conn.LessonParticipants.Where(p => p.Account_ID == accountId)
                                .Select(p => p.Lesson)
                                .OrderBy(p => p.Subject)
                                .ToList();

                        foreach (var lesson in ret.lessons)
                        {
                            if (lesson.BookingDate.Date >= DateTime.Now.Date && lesson.BookingDate.Date <= DateTime.Now.Date.AddDays(1))
                            {
                                ret.today_lessons.Add(lesson);
                            }
                        }
                        break;

                    case Types.AccountTypes.Teacher:
                        ret.classrooms = new List<ViewModels.Homepage.Dashboard.classroom>();
                        ret.lessons = conn.Lessons.Where(l => l.Account_ID == accountId)
                            .ToList();

                        foreach (var lesson in ret.lessons)
                        {
                            if (lesson.BookingDate.Date >= DateTime.Now.Date && lesson.BookingDate.Date <= DateTime.Now.Date.AddDays(1))
                            {
                                ret.classrooms.Add(new ViewModels.Homepage.Dashboard.classroom
                                {
                                    lesson = lesson,
                                    participants = conn.LessonParticipants.Where(l => l.Lesson_ID == lesson.ID)
                                    .ToList()
                                });
                            }
                        }
                        break;
                }

                return ret;
            }
        }

        public ViewModels.Homepage.Landing Signup(int type, string firstname, string lastname, string email, string password)
        {
            var ret = new ViewModels.Homepage.Landing()
            {
                firstName = firstname.Trim(),
                lastName = lastname.Trim(),
                email = email.Trim(),
                password = password.Trim()
            };

            Rules.Signup.Register(type, firstname, lastname, email, password);

            return ret;
        }

        public ViewModels.Homepage.SubjectList LoadSubjectList(string subject, int? accountId)
        {
            var ret = new ViewModels.Homepage.SubjectList()
            {
                subject = subject.Replace("_", " ")
            };

            using (var conn = new dbEntities())
            {
                ret.subjectDescription = conn.SubjectDescriptions.FirstOrDefault(s => s.Subject == ret.subject) ??
                                          new SubjectDescription()
                                          {
                                              Subject = subject
                                          };
                ret.ExamPapers = conn.SubjectExamPapers.Where(s => s.SubjectDescription_ID == ret.subjectDescription.ID).ToList();

                var rs = conn.Lessons
                            .Where(l =>
                                l.Subject == ret.subject &&
                                l.Hidden != true &&
                                !l.CancelledDate.HasValue &&
                                (l.ClassSize == 0 || l.ClassSize > l.LessonParticipants.Count));

                ret.lessons = new List<Lesson>();
                var teacherType = (int)Constants.Types.AccountTypes.Teacher;
                var disabled = Constants.Status.Disabled;
                var approved = Constants.Status.Approved;
                var hidden = Constants.Status.Hidden;
                var tutors = conn.Accounts.Where(t => t.AccountType_ID == teacherType && (t.Approved == approved && t.Disabled != disabled))
                    .ToList();

                ret.lessons = accountId.HasValue 
                    ? rs.Where(l => l.LessonParticipants.All(p => p.Account_ID != accountId) && l.Hidden != hidden).ToList() 
                    : rs.Where(l => l.Hidden != hidden).ToList();

                foreach (var lesson in ret.lessons)
                {
                    foreach (var tutor in tutors)
                    {
                        if (lesson.Account_ID == tutor.ID)
                        {
                            lesson.BookingDate = logic.Rules.Timezone.ConvertFromUTC(lesson.BookingDate);

                            ret.classList.Add(new ViewModels.Homepage.SubjectList.virtRoom
                            {
                                lesson = lesson,
                                tutor = conn.Accounts.FirstOrDefault(t => t.ID == lesson.Account_ID)
                            });

                            break;
                        }
                    }
                }
            }

            return ret;
        }

        public ViewModels.Homepage.SubjectList LoadSubjectListByTeacher(int teacherId)
        {
            using (var conn = new dbEntities())
            {
                return new ViewModels.Homepage.SubjectList()
                {
                    tutor = conn.Accounts.FirstOrDefault(a => a.ID == teacherId) ?? new Account(),
                    lessons = conn.Lessons
                        .Where(l =>
                            l.Account_ID == teacherId &&
                            l.Hidden != true &&
                            !l.CancelledDate.HasValue &&
                            (l.ClassSize == 0 || l.ClassSize > l.LessonParticipants.Count))
                        .ToList()
                };
            }
        }

        public ViewModels.Homepage.TeacherList LoadTeacherList(string subject)
        {
            using (var conn = new dbEntities())
            {
                return new ViewModels.Homepage.TeacherList()
                {
                    //Note to add ability to search Teachers by subject.
                    teachers = conn.Accounts
                        .Where(l => l.AccountType_ID == Constants.Types.Teacher && (l.Approved == true && l.Disabled == false))
                        .ToList()
                };
            }
        }

        public List<ApiViewModels.DashBoard.GetDay.calEvent> GDay(int userID, string date)
        {
            var dateVal = Rules.Timezone.ConvertToUTC(Convert.ToDateTime(date));

            var calEvents = new List<ApiViewModels.DashBoard.GetDay.calEvent>();
            var tomorrow = dateVal.AddDays(1);
            using (var conn = new dbEntities())
            {
                var user = conn.Accounts.FirstOrDefault(a => a.ID == userID);
                if (user == null)
                    throw new Exception("User not found.");

                switch (user.AccountType_ID)
                {
                    case 1: // Student
                        var lessons = conn.LessonParticipants
                                        .Where(p => p.Account_ID == userID)
                                        .Select(p => p.Lesson)
                                        .Where(l => l.BookingDate >= dateVal && l.BookingDate < tomorrow)
                                        .ToList();
                        foreach (var l in lessons)
                        {
                            var duration = Convert.ToDouble(l.DurationMins);

                            calEvents.Add(new ApiViewModels.DashBoard.GetDay.calEvent
                            {
                                id = Convert.ToString(l.ID),
                                title = String.Concat(l.Account.Fname, " ", l.Account.Lname, " - ", l.Description),
                                start = Timezone.ConvertFromUTC(l.BookingDate).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"),
                                end = Timezone.ConvertFromUTC(l.BookingDate).AddMinutes(duration).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"),
                                color = Constants.Colors.STUDENT
                            });

                        }
                        break;

                    case 2: // Teacher
                        var ownLessons = conn.Lessons
                            .Where(p => p.Account_ID == userID)
                            .Where(l => l.BookingDate >= dateVal && l.BookingDate < tomorrow)
                            .ToList();

                        foreach (var l in ownLessons)
                        {
                            var duration = Convert.ToDouble(l.DurationMins);

                            calEvents.Add(new ApiViewModels.DashBoard.GetDay.calEvent
                            {
                                id = Convert.ToString(l.ID),
                                title = String.Concat(l.Account.Fname, " ", l.Account.Lname, " - ", l.Description),
                                start = Timezone.ConvertFromUTC(l.BookingDate).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"),
                                end = Timezone.ConvertFromUTC(l.BookingDate).AddMinutes(duration).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"),
                                color = Constants.Colors.TEACHER
                            });

                        }

                        goto case 1;
                }

                return calEvents;
            }
        }

        public List<ApiViewModels.DashBoard.GetEvents.calEvent> GetLessons(int userID)
        {
            var calEvents = new List<ApiViewModels.DashBoard.GetEvents.calEvent>();

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
                            if (l.BookingDate >= DateTime.Now)
                            {
                                calEvents.Add(new ApiViewModels.DashBoard.GetEvents.calEvent
                                {
                                    id = l.ID,
                                    title = l.Subject,
                                    start = Timezone.ConvertFromUTC(l.BookingDate).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"),
                                    color = Constants.Colors.STUDENT

                                });
                            }
                            else
                            {
                                calEvents.Add(new ApiViewModels.DashBoard.GetEvents.calEvent
                                {
                                    id = l.ID,
                                    title = l.Subject,
                                    start = Timezone.ConvertFromUTC(l.BookingDate).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"),
                                    color = Constants.Colors.PAST

                                });
                            }
                        }

                        break;

                    case 2: // Teacher
                        var ownLessons = conn.Lessons.Where(p => p.Account_ID == userID)
                            .ToList();

                        foreach (var l in ownLessons)
                        {
                            if (l.BookingDate >= DateTime.Now)
                            {
                                calEvents.Add(new ApiViewModels.DashBoard.GetEvents.calEvent
                                {
                                    id = l.ID,
                                    title = l.Subject,
                                    start = Timezone.ConvertFromUTC(l.BookingDate).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"),
                                    color = Constants.Colors.TEACHER

                                });
                            }
                            else
                            {
                                calEvents.Add(new ApiViewModels.DashBoard.GetEvents.calEvent
                                {
                                    id = l.ID,
                                    title = l.Subject,
                                    start = Timezone.ConvertFromUTC(l.BookingDate).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"),
                                    color = Constants.Colors.PAST

                                });
                            }
                        }

                        goto case 1;
                }

                return calEvents;
            }
        }
    }
}
