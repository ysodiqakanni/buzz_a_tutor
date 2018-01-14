using bat.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace bat.logic.Services
{
    public class Admin : _ServiceClassBaseMarker
    {
        public ViewModels.Admin.Edit LoadEditViewModel(int id)
        {
            using (var conn = new dbEntities())
            {
                var ret = new ViewModels.Admin.Edit()
                {
                    subjectDescription = conn.SubjectDescriptions.FirstOrDefault(s => s.ID == id)
                };
                if (ret.subjectDescription == null)
                    throw new Exception("Invalid subject description record.");

                ret.ExamPapers = conn.SubjectExamPapers.Where(s => s.SubjectDescription_ID == ret.subjectDescription.ID).ToList();
                ret.subject = ret.subjectDescription.Subject;

                return ret;
            }
        }

        public ViewModels.Admin.Edit LoadEditViewModel(string subject)
        {
            var ret = new ViewModels.Admin.Edit()
            {
                subject = subject.Replace("_", " ").Trim()
            };
            
            var matched = false;
            foreach (var dpt in bat.logic.Constants.Subjects.Departments)
            {
                if (dpt.Subjects.Contains(ret.subject))
                {
                    matched = true;
                }
            }

            if (!matched)
                throw new Exception("Invalid subject");

            using (var conn = new dbEntities())
            {
                ret.subjectDescription = conn.SubjectDescriptions.FirstOrDefault(s => s.Subject == subject) ??
                                          new SubjectDescription()
                                          {
                                              Subject = subject
                                          };
                if (ret.subjectDescription.ID > 0)
                    ret = LoadEditViewModel(ret.subjectDescription.ID);
            }

            return ret;
        }
        
        public void Save(string subject, string description)
        {
            using (var conn = new dbEntities())
            {
                var sd = conn.SubjectDescriptions.FirstOrDefault(s => s.Subject == subject) ??
                                          new SubjectDescription()
                                          {
                                              Subject = subject
                                          };
                sd.Description = HttpUtility.UrlEncode((description ?? "").Trim());

                if (sd.ID == 0)
                    conn.SubjectDescriptions.Add(sd);

                conn.SaveChanges();
            }
        }

        public string UploadPaper(int subjectDescriptionId, HttpPostedFileBase paper)
        {
            try
            {
                using (var conn = new dbEntities())
                {
                    conn.SubjectExamPapers.Add(new SubjectExamPaper()
                    {
                        SubjectDescription_ID = subjectDescriptionId,
                        StorageName = logic.Helpers.AzureStorage.StoredResources.UploadExamPaper(paper),
                        Original_Name = paper.FileName,
                    });

                    conn.SaveChanges();
                }
                return paper.FileName;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void DeletePaper(int paperId)
        {
            using (var conn = new dbEntities())
            {
                var paper = conn.SubjectExamPapers.FirstOrDefault(s => s.ID == paperId);
                if (paper == null) throw new Exception("The exam paper does not exist.");

                bat.logic.Helpers.AzureStorage.AzureBlobStorage.Delete(bat.logic.Constants.Azure.AZURE_UPLOADED_EXAM_PAPERS_STORAGE_CONTAINER, paper.StorageName);
                conn.SubjectExamPapers.Remove(conn.SubjectExamPapers.FirstOrDefault(s => s.ID == paperId));
                conn.SaveChanges();
            }
        }

        public ViewModels.Admin.Lesson LoadLesson(int id)
        {
            using (var conn = new dbEntities())
            {
                var ret = new ViewModels.Admin.Lesson()
                {
                    lesson = conn.Lessons.FirstOrDefault(l => l.ID == id)
                };
                if (ret.lesson == null)
                    throw new Exception("Lesson not found");

                ret.participants = conn.LessonParticipants.Where(p => p.Lesson_ID == id).ToList();

                return ret;
            }
        }

        public void SetLessonVisibility(int id, bool status)
        {
            using (var conn = new dbEntities())
            {
                var lesson = conn.Lessons.FirstOrDefault(l => l.ID == id);
                if (lesson == null) throw new Exception("Lesson record does not exist.");

                lesson.Hidden = status;
                conn.SaveChanges();
            }
        }


        public ViewModels.Admin.Lessons LoadLessons()
        {
            using (var conn = new dbEntities())
            {
                return new ViewModels.Admin.Lessons()
                {
                    lessons = conn.Lessons
                        .Where(l => l.BookingDate >= DateTime.UtcNow)
                        .OrderBy(l => l.BookingDate)
                        .ToList()
                };
            }
        }

        public void SetAccountStatus(int id, bool status)
        {
            using (var conn = new dbEntities())
            {
                var tutor = conn.Accounts.FirstOrDefault(l => l.ID == id);
                if (tutor == null) throw new Exception("Account does not exist.");

                tutor.Disabled = status;
                conn.SaveChanges();
            }
        }

        public ViewModels.Admin.Students LoadStudents()
        {
            using (var conn = new dbEntities())
            {
                return new ViewModels.Admin.Students()
                {
                    students = conn.Accounts.Where(l => l.AccountType_ID == 1).ToList()
                };
            }
        }

        public ViewModels.Admin.TeacherProfile LoadTeacherProfile(int id)
        {
            using (var conn = new dbEntities())
            {
                var ret = new ViewModels.Admin.TeacherProfile()
                {
                    teacher = conn.Accounts.FirstOrDefault(t => t.ID == id)
                };

                if (ret.teacher == null)
                    throw new Exception("Teacher not found");

                return ret;
            }
        }

        public ViewModels.Admin.Tutors LoadTutors()
        {
            using (var conn = new dbEntities())
            {
                return new ViewModels.Admin.Tutors()
                {
                    tutors = conn.Accounts.Where(l => l.AccountType_ID == 2).ToList()
                };
            }
        }

        public void ApproveAccount(int id, bool status)
        {
            using (var conn = new dbEntities())
            {
                var tutor = conn.Accounts.FirstOrDefault(l => l.ID == id);
                if (tutor == null) throw new Exception("Account does not exist.");

                tutor.Approved = status;
                conn.SaveChanges();

                Helpers.Emailer.SendPlainText(
                Constants.Email.Address, Constants.Email.Name,
                tutor.Email,
                "Buzz a Tutor - Account has been approved",
                "Congratulations " + tutor.Fname + " " + tutor.Lname + ", your account has been approved. You are now able to create and host Lessons.");
            }
        }
    }
}
