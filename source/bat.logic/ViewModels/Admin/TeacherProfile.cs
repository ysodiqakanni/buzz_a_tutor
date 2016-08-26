using bat.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.ViewModels.Admin
{
    public class TeacherProfile : Master
    {
        public Account teacher { get; set; }
        public void Load (int id)
        {
            using (var conn = new dbEntities())
            {
                this.teacher = conn.Accounts.FirstOrDefault(t => t.ID == id);
                if (teacher == null)
                    throw new Exception("Teacher not found");
            }
        }
    }
}
