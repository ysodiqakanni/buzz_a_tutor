using bat.data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace bat.logic.ApiModels.Lessons
{
    public class Upload
    {
        public class lessonAttachment
        {
            public int id { get; set; }
            public string title { get; set; }
        }

        public List<lessonAttachment> lessonAttachments { get; set; }

        public static string UploadImage(int lessonId, int accId, string title, string data)
        {
            var json = new JavaScriptSerializer();
            var lessonAttachments = new List<lessonAttachment>();
            try
            {
                using (var conn = new dbEntities())
                {
                    conn.LessonAttachments.Add(new LessonAttachment()
                    {
                        Lesson_ID = lessonId,
                        Account_ID = accId,
                        Title = title,
                        Data = data
                    });
                    conn.SaveChanges();

                    var attachments = conn.LessonAttachments
                        .Where(l => l.Lesson_ID == lessonId)
                        .ToList();

                    foreach(var attachment in attachments)
                    {
                        lessonAttachments.Add(new lessonAttachment()
                        {
                            id = attachment.ID,
                            title = attachment.Title
                        });
                    }
                    return json.Serialize(lessonAttachments);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
