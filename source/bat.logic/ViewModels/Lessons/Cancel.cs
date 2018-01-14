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

namespace bat.logic.ViewModels.Lessons
{
    public class Cancel
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
    }
}