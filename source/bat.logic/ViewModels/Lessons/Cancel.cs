using bat.data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using bat.logic.Exceptions;

namespace bat.logic.ViewModels.Lessons
{
    public class Cancel : Master
    {

        public Account host { get; set; }

        public Lesson lesson { get; set; }


        public Cancel()
        {
            this.lesson = new Lesson()
            {
                BookingDate = DateTime.UtcNow,
            };
            this.host = new Account();
        }

        public void Load(int id)
        {
            using (var conn = new dbEntities())
            {
                this.lesson = conn.Lessons.FirstOrDefault(l => l.ID == id);
                if (this.lesson == null) throw new Exception("Lesson does not exist.");
                if (this.lesson.Account_ID != this.account.ID) throw new WrongAccountException();

                this.host = this.lesson.Account;

                // timezone out for displaying
                this.lesson.BookingDate = Rules.Timezone.ConvertFromUTC(this.lesson.BookingDate);

                this.lesson.Subject = this.lesson.Subject ?? "";
            }
        }

        public void Save(int lessonId, FormCollection frm)
        {
            using (var conn = new dbEntities())
            {
                this.lesson = conn.Lessons.FirstOrDefault(l => l.ID == lessonId);
                if (this.lesson == null) throw new Exception("Lesson does not exist.");

                this.lesson.CancelledDate = DateTime.UtcNow;
                conn.SaveChanges();

                Rules.EventLogging.EditLesson(this.account, this.lesson);
            }
        }
    }
}