using bat.data;
using bat.logic.Exceptions;
using EntityFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace bat.logic.Services
{
    public class Profile : _ServiceClassBaseMarker
    {
        public ViewModels.Profile.Edit LoadEdit(int id)
        {
            var ret = new ViewModels.Profile.Edit();

            using (var conn = new dbEntities())
            {
                ret.account = conn.Accounts.FirstOrDefault(a => a.ID == id);
                if (ret.account == null) throw new Exception("Account does not exist.");
            }

            return ret;
        }

        public ViewModels.Profile.Edit ProfileEditSave(
            int id, string firstName, string lastName, string description, string qualifications, int? rate, 
            System.Web.HttpPostedFileBase picture)
        {
            var ret = new ViewModels.Profile.Edit();

            using (var conn = new dbEntities())
            {
                ret.account = conn.Accounts.FirstOrDefault(a => a.ID == id);
                if (ret.account == null) throw new Exception("Account does not exist.");

                ret.account.Fname = firstName;
                ret.account.Lname = lastName;
                ret.account.Description = description;
                ret.account.Qualifications = qualifications;
                ret.account.Rate = rate;

                if (picture != null)
                {
                    if (ret.account.Picture == null)
                    {
                        ret.account.Picture = Helpers.AzureStorage.StoredResources.UploadProfilePicture(picture);
                    }
                    else
                    {
                        Helpers.AzureStorage.AzureBlobStorage.Delete(Constants.Azure.AZURE_UPLOADED_PROFILE_IMAGES_STORAGE_CONTAINER, ret.account.Picture);
                        ret.account.Picture = Helpers.AzureStorage.StoredResources.UploadProfilePicture(picture);
                    }
                }

                conn.SaveChanges();
            }

            return ret;
        }

        public ViewModels.Profile.New SaveNew(int id, FormCollection frm)
        {
            var ret = new ViewModels.Profile.New();

            using (var conn = new dbEntities())
            {
                var firstName = (frm["Fname"] ?? "").Trim();
                var lastName = (frm["Lname"] ?? "").Trim();
                var email = (frm["Email"] ?? "").Trim();

                if (string.IsNullOrEmpty(firstName))
                    throw new Exception("First name required.");

                if (string.IsNullOrEmpty(lastName))
                    throw new Exception("Last name required.");

                if (string.IsNullOrEmpty(email))
                    throw new Exception("Email required.");

                if (!Helpers.Strings.EmailValid(email))
                    throw new Exception("Email not valid.");

                var accountDetails = new Account()
                {
                    AccountType_ID = (int)Constants.Types.AccountTypes.Student,
                    Fname = firstName,
                    Lname = lastName,
                    Email = email,
                    Password = Helpers.PasswordStorage.CreateHash("")
                };
                conn.Accounts.Add(accountDetails);
                conn.SaveChanges();

                ret.familyMemeber = new FamilyMember()
                {
                    Account_ID = accountDetails.ID,
                    Parent_ID = id,
                };
                conn.FamilyMembers.Add(ret.familyMemeber);
                conn.SaveChanges();
            }

            return ret;
        }

        public ViewModels.Profile.EditMember LoadEditMember(int id, int loggedInUserId)
        {
            var ret = new ViewModels.Profile.EditMember();

            using (var conn = new dbEntities())
            {
                ret.familyMember = conn.FamilyMembers.AsNoTracking().FirstOrDefault(a => a.ID == id);
                if (ret.familyMember == null) throw new Exception("Family member does not exist.");
                if (ret.familyMember.Parent_ID != loggedInUserId) throw new WrongAccountException();

                ret.FamilyMemberAccount = conn.Accounts.AsNoTracking().FirstOrDefault(a => a.ID == ret.familyMember.Account_ID) ?? new Account();
            }

            return ret;
        }
        
        public ViewModels.Profile.EditMember SaveEditMember(FormCollection frm, int loggedInUserId)
        {
            var ret = LoadEditMember(Convert.ToInt32((frm["MemberID"])), loggedInUserId);

            using (var conn = new dbEntities())
            {
                var firstName = (frm["Fname"] ?? "").Trim();
                var lastName = (frm["Lname"] ?? "").Trim();
                var email = (frm["Email"] ?? "").Trim();

                if (string.IsNullOrEmpty(firstName))
                    throw new Exception("First name required.");

                if (string.IsNullOrEmpty(lastName))
                    throw new Exception("Last name required.");

                if (string.IsNullOrEmpty(email))
                    throw new Exception("Email required.");

                if (!Helpers.Strings.EmailValid(email))
                    throw new Exception("Email not valid.");

                var accountDetails = conn.Accounts.FirstOrDefault(a => a.ID == ret.familyMember.Account_ID);
                if (accountDetails == null) throw new Exception("Account does not exist.");

                accountDetails.Fname = firstName;
                accountDetails.Lname = lastName;
                accountDetails.Email = email;

                conn.SaveChanges();
            }

            return ret;
        }

        public ViewModels.Profile.EditMember DeleteEditMember(int id, int loggedInUserId)
        {
            var ret = LoadEditMember(id, loggedInUserId);

            using (var conn = new dbEntities())
            {
                var member = conn.FamilyMembers.FirstOrDefault(i => i.ID == id);
                if (member != null) conn.FamilyMembers.Remove(member);
                conn.Accounts.Where(a => a.ID == ret.familyMember.Account_ID).Delete();
                conn.SaveChanges();
            }

            return ret;
        }

        public ViewModels.Profile.LessonDetails LoadLessonDetails(int id, int loggedInUserId)
        {
            var ret = new ViewModels.Profile.LessonDetails();

            using (var conn = new dbEntities())
            {
                ret.lesson = conn.Lessons.AsNoTracking().FirstOrDefault(l => l.ID == id);
                if (ret.lesson == null) throw new Exception("Lesson does not exist.");
                if (ret.lesson.Account_ID != loggedInUserId) throw new WrongAccountException();

                // timezone out for displaying
                ret.lesson.BookingDate = Rules.Timezone.ConvertFromUTC(ret.lesson.BookingDate);

                foreach (var participant in ret.lesson.LessonParticipants.ToList())
                {
                    var other = conn.Accounts.FirstOrDefault(a => a.ID == participant.Account_ID);
                    if (other != null)
                        ret.others.Add(other);
                }

                ret.chatRecords = ret.lesson.ChatRecords.ToList();
            }

            return ret;
        }

        public ViewModels.Profile.Lessons LoadLessons(int id)
        {
            var ret = new ViewModels.Profile.Lessons();

            using (var conn = new dbEntities())
            {
                ret.lessons = new List<Lesson>();
                ret.familyRecords = new List<ViewModels.Profile.Lessons.FamilyRecord>();

                var parentLessons = conn.Lessons
                    .Join(conn.LessonParticipants,
                        l => l.ID, // lesson primary key
                        p => p.Lesson_ID, // equals participant foreign key
                        (l, p) => new { l, p })
                    .Select(s => new
                    {
                        s.l,
                        participant = s.p.Account_ID
                    })
                    .Where(l => l.participant == id);

                foreach (var l in parentLessons.ToList())
                {
                    ret.lessons.Add(l.l);
                }

                ret.teacherLessons = conn.Lessons.Where(p => p.Account_ID == id).ToList();

                var familyMembers = conn.FamilyMembers.Where(p => p.Parent_ID == id).ToList();

                foreach (var familyMember in familyMembers.Select(a => new { a.Account_ID }).ToList())
                {
                    var lessons = new List<Lesson>();

                    var account = conn.Accounts.FirstOrDefault(a => a.ID == familyMember.Account_ID);
                    if (account == null) throw new Exception("Account does not exist.");

                    var childLessons = conn.Lessons
                        .Join(conn.LessonParticipants,
                            l => l.ID, // lesson primary key
                            p => p.Lesson_ID, // equals participant foreign key
                            (l, p) => new { l, p })
                        .Select(s => new
                        {
                            s.l,
                            participant = s.p.Account_ID
                        })
                        .Where(l => l.participant == familyMember.Account_ID);

                    foreach (var l in childLessons.ToList())
                    {
                        lessons.Add(l.l);
                    }

                    ret.familyRecords.Add(new ViewModels.Profile.Lessons.FamilyRecord()
                    {
                        familyMember = account,
                        lessons = lessons
                    });
                }
            }

            return ret;
        }

        public string GetProfilePicture(int accountId)
        {
            using (var conn = new dbEntities())
            {
                var account = conn.Accounts.FirstOrDefault(a => a.ID == accountId);
                if (account == null)
                    throw new Exception("Account not found.");

                return account.Picture;
            }
        }
    }
}
