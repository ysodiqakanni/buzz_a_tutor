using bat.data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.ApiModels.Lessons
{
    public class GetAttachment
    {
        public static string GetImageStream(int attachmentId)
        {
            using (var conn = new dbEntities())
            {
                var attachment = conn.LessonAttachments.FirstOrDefault(a => a.ID == attachmentId);
                if (attachment == null)
                    throw new Exception("Attachment not found.");

                return attachment.Data;
            }
        }
    }
}
