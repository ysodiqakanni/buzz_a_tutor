using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using bat.logic.Exceptions;

namespace bat.logic.ViewModels.Profile
{
    public class LessonDetails : Master
    {
        public List<ChatRecord> chatRecords { get; set; }
        public Lesson lesson { get; set; }
        public List<Account> others { get; set; }

        public LessonDetails()
        {
            this.others = new List<Account>();
            this.chatRecords = new List<ChatRecord>();
        }

        public void Load(int id)
        {
            using (var conn = new dbEntities())
            {
                this.lesson = conn.Lessons.FirstOrDefault(l => l.ID == id);
                if (this.lesson == null) throw new Exception("Lesson does not exist.");
                if (this.lesson.Account_ID != this.account.ID) throw new WrongAccountException();

                // timezone out for displaying
                this.lesson.BookingDate = Rules.Timezone.ConvertFromUTC(this.lesson.BookingDate);
                
                foreach (var participant in this.lesson.LessonParticipants.ToList())
                {
                    var other = conn.Accounts.FirstOrDefault(a => a.ID == participant.Account_ID);
                    if (other != null)
                        this.others.Add(other);
                }

                this.chatRecords = this.lesson.ChatRecords.ToList();
            }
        }
    }
}