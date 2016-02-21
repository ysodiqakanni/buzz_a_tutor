using bat.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace bat.logic.Models.Lessons
{
    public class View : Master
    {
        public Lesson lesson { get; set; }

        public View()
        {
            this.lesson = new Lesson()
            {
                BookingDate = Shearnie.Net.OzTime.GetNowAEST(),
                DurationMins = 15,
                ClassSize = 1
            };
        }

        public bool IsTeacher =>
            this.account.AccountType_ID == (int)bat.logic.Constants.Types.AccountTypes.Teacher;

        public void Load(int id)
        {
            using (var conn = new dbEntities())
            {
                this.lesson = conn.Lessons.FirstOrDefault(l => l.ID == id);
                if (this.lesson == null) throw new Exception("Lesson does not exist.");
            }
        }
    }
}
