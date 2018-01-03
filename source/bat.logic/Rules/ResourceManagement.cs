using bat.data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace bat.logic.Rules
{
    public class ResourceManagement
    {
        public static string UploadResource(int lessonId, HttpPostedFileBase resource)
        {
            try
            {
                using (var conn = new dbEntities())
                {
                    var lesson = conn.Lessons.FirstOrDefault(l => l.ID == lessonId);
                    if (lesson == null) throw new Exception("Lesson does not exist.");

                    var lessonResource = new LessonResource()
                    {
                        Lession_ID = lessonId,
                        Original_Name = resource.FileName,
                        Item_Storage_Name = logic.Helpers.AzureStorage.StoredResources.UploadLessonResource(resource),
                        Type_ID = Constants.Types.Default
                    };
                    conn.LessonResources.Add(lessonResource);

                    conn.SaveChanges();
                }
                return resource.FileName;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string UploadResourceImage(FormDataCollection resourceImage)
        {
            try
            {
                var lessonId = Convert.ToInt32(resourceImage["lessonId"]);
                using (var conn = new dbEntities())
                {
                    var lesson = conn.Lessons.FirstOrDefault(l => l.ID == lessonId);
                    if (lesson == null) throw new Exception("Lesson does not exist.");
                    int lessonResourceId = 0;

                    if (lesson.LessonResources.Count > 0)
                    {
                        var lessonResource = lesson.LessonResources.OrderByDescending(r=>r.ID).FirstOrDefault();
                        lessonResource.Lession_ID = lessonId;
                        lessonResourceId = lessonResource.ID;
                        lessonResource.Original_Name = resourceImage["title"];
                        lessonResource.Item_Storage_Name = logic.Helpers.AzureStorage.StoredResources.UploadLessonResourceImage(resourceImage["data"]);
                        lessonResource.Type_ID = Constants.Types.Image;
                    }

                    if (lessonResourceId == 0)
                    {
                        var lessonResource = new LessonResource()
                        {
                            Lession_ID = lessonId,
                            Original_Name = resourceImage["title"],
                            Item_Storage_Name = logic.Helpers.AzureStorage.StoredResources.UploadLessonResourceImage(resourceImage["data"]),
                            Type_ID = Constants.Types.Image
                        };

                        conn.LessonResources.Add(lessonResource);
                    }

                    conn.SaveChanges();
                }
                return resourceImage["title"];
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static MemoryStream DownloadLessonResource(int resourseID)
        {
            try
            {
                using (var conn = new dbEntities())
                {
                    LessonResource resource = new LessonResource();
                    resource = conn.LessonResources.FirstOrDefault(r => r.ID == resourseID);
                    if (resource == null)
                        throw new Exception("Lesson resource not found.");

                    using (var memoryStream = new MemoryStream())
                    {
                        logic.Helpers.AzureStorage.StoredResources.DownloadLessonResource(memoryStream, resource.Item_Storage_Name);
                        return memoryStream;
                    }
                }
            }
            catch (Exception ex)
            {
                var ignore = ex;
            }
            return null;
        }

        public static string DownloadLessonImage(int resourseID)
        {
            try
            {
                using (var conn = new dbEntities())
                {
                    LessonResource resource = new LessonResource();
                    resource = conn.LessonResources.FirstOrDefault(r => r.ID == resourseID);
                    if (resource == null)
                        throw new Exception("Lesson resource not found.");

                    using (var memoryStream = new MemoryStream())
                    {
                        logic.Helpers.AzureStorage.StoredResources.DownloadLessonResource(memoryStream, resource.Item_Storage_Name);
                        var imageByte = memoryStream.ToArray();

                        return System.Text.Encoding.UTF8.GetString(imageByte, 0, imageByte.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                var ignore = ex;
            }
            return null;
        }

        public static string DeleteResource(int resourceID)
        {
            using (var conn = new dbEntities())
            {
                LessonResource resource = new LessonResource();

                resource = conn.LessonResources.FirstOrDefault(a => a.ID == resourceID);
                if (resource == null) throw new Exception("The resource does not exist.");

                bat.logic.Helpers.AzureStorage.AzureBlobStorage.Delete(bat.logic.Constants.Azure.AZURE_UPLOADED_LESSON_RESOURCES_STORAGE_CONTAINER, resource.Item_Storage_Name);
                conn.LessonResources.Remove(conn.LessonResources.FirstOrDefault(i => i.ID == resourceID));
                conn.SaveChanges();
            }
            return null;
        }

        public static MemoryStream DownloadProfilePicture(int accountId)
        {
            try
            {
                using (var conn = new dbEntities())
                {
                    var account = new Account();

                    account = conn.Accounts.FirstOrDefault(r => r.ID == accountId);
                    if (account == null)
                        throw new Exception("Account not found.");

                    using (var memoryStream = new MemoryStream())
                    {
                        if (account.Picture == null)
                        {
                            return null;
                        }
                        else
                        {
                            logic.Helpers.AzureStorage.StoredResources.DownloadProfilePicture(memoryStream, account.Picture);
                        }
                        return memoryStream;
                    }
                }
            }
            catch (Exception ex)
            {
                var ignore = ex;
                return null;
            }
        }
    }
}
