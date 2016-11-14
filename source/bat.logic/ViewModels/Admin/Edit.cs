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
        public List<SubjectExamPaper> ExamPapers { get; set; }


        public Edit()
        {
            subjectDescription = new SubjectDescription();
            ExamPapers = new List<SubjectExamPaper>();
        }

        public void Load(int id)
        {
            using (var conn = new dbEntities())
            {
                this.subjectDescription = conn.SubjectDescriptions.FirstOrDefault(s => s.ID == id);
                if (this.subjectDescription == null)
                    throw new Exception("Invalid subject description record.");
                this.ExamPapers = conn.SubjectExamPapers.Where(s => s.SubjectDescription_ID == this.subjectDescription.ID).ToList();

                this.subject = this.subjectDescription.Subject;
            }
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
                if (this.subjectDescription.ID > 0)
                    Load(this.subjectDescription.ID);
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

        public string UploadPaper(HttpPostedFileBase paper)
        {
            try
            {
                using (var conn = new dbEntities())
                {
                    conn.SubjectExamPapers.Add(new SubjectExamPaper()
                    {
                        SubjectDescription_ID = this.subjectDescription.ID,
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
    }
}
