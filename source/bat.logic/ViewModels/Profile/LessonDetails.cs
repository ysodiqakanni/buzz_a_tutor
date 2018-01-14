using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using bat.logic.Exceptions;

namespace bat.logic.ViewModels.Profile
{
    public class LessonDetails
    {
        public List<ChatRecord> chatRecords { get; set; }
        public Lesson lesson { get; set; }
        public List<Account> others { get; set; }

        public LessonDetails()
        {
            this.others = new List<Account>();
            this.chatRecords = new List<ChatRecord>();
        }
    }
}