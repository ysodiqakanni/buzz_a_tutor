using bat.data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.ApiModels.Lessons
{
    public class Upload
    {
        public static string UploadImage(int lessonId, int accId, string title, string data)
        {
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
                }

                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
