using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using bat.logic.Constants;
using bat.logic.Exceptions;

namespace bat.logic.Models.Records
{
    public class Details : Master
    {
        public ChatRecord chatrecord { get; set; }
        public List<ChatRecord> chatRecords { get; set; }
        public Lesson lesson { get; set; }
        public List<Lesson> lessons { get; set; }

        public void Load(int id)
        {
            this.chatRecords = new List<ChatRecord>();
            using (var conn = new dbEntities())
            {
                this.lesson = conn.Lessons.FirstOrDefault(a => a.ID == id);
                if (this.lesson == null) throw new Exception("Lesson does not exist.");

                foreach (var lesson in this.lessons = conn.Lessons.Where(p => p.ID == id).ToList())
                {
                    this.chatRecords = conn.ChatRecords.Where(l => l.Lesson_ID == lesson.ID).ToList();
                }
            }
        }
    }
}