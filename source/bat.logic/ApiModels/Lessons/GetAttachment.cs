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
    public class GetAttachment
    {
        public class blackboardImage
        {
            public string id { get; set; }
            public string title { get; set; }
        }

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

        public static string GetBlackboardImages(int lessonId)
        {
            using (var conn = new dbEntities())
            {
                var json = new JavaScriptSerializer();
                var bbImageList = new blackboardImage();
                try
                {
                    //fetch last attached resorce with lesson
                    var resources = conn.LessonResources.Where(a => a.Lession_ID == lessonId && a.Type_ID == Constants.Types.Image).OrderByDescending(rid => rid.ID).FirstOrDefault();
                    if (resources == null)
                        throw new Exception("Attachment not found.");
                    bbImageList.id = Convert.ToString(resources.ID);
                    bbImageList.title = resources.Original_Name;
                }
                catch (Exception)
                {

                    //throw;
                }
                return json.Serialize(bbImageList);
            }
        }
    }
}
