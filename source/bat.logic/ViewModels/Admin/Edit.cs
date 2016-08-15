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

namespace bat.logic.ViewModels.Admin
{
    public class Edit : Master
    {
        public string subject { get; set; }
        public SubjectDescription subjectDescription { get; set; }
        
        public Edit()
        {
        }

        public void Load(string subject)
        {
            this.subject = subject.Replace("_", " ").Trim();

            var matched = false;
            foreach (var dpt in bat.logic.Constants.Subjects.Departments)
            {
                if (dpt.Subjects.Contains(this.subject))
                {
                    matched = true;
                }
            }

            if (!matched)
                throw new Exception("Invalid subject");

            using (var conn = new dbEntities())
            {
                this.subjectDescription = conn.SubjectDescriptions.FirstOrDefault(s => s.Subject == subject) ??
                                          new SubjectDescription()
                                          {
                                              Subject = subject
                                          };
            }
        }

        public void Save(string description)
        {
            using (var conn = new dbEntities())
            {
                var sd = conn.SubjectDescriptions.FirstOrDefault(s => s.Subject == this.subject) ??
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
    }
}
