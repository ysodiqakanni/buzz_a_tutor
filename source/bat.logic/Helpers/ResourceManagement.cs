using bat.data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace bat.logic.Helpers
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
                        Item_Storage_Name = logic.Helpers.AzureStorage.StoredResources.Upload(resource)
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
                        bat.logic.Helpers.AzureStorage.AzureBlobStorage.Download(bat.logic.Constants.Azure.AZURE_UPLOADED_IMAGES_STORAGE_CONTAINER, resource.Item_Storage_Name).DownloadToStream(memoryStream);
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

        public static string DeleteResource(int resourceID)
        {
            using (var conn = new dbEntities())
            {
                LessonResource resource = new LessonResource();

                resource = conn.LessonResources.FirstOrDefault(a => a.ID == resourceID);
                if (resource == null) throw new Exception("The resource does not exist.");

                bat.logic.Helpers.AzureStorage.AzureBlobStorage.Delete(bat.logic.Constants.Azure.AZURE_UPLOADED_IMAGES_STORAGE_CONTAINER, resource.Item_Storage_Name);
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
                        if(account.Picture == null)
                        {
                            bat.logic.Helpers.AzureStorage.AzureBlobStorage.Download(bat.logic.Constants.Azure.AZURE_UPLOADED_IMAGES_STORAGE_CONTAINER, bat.logic.Constants.Picture.Default_Image).DownloadToStream(memoryStream);
                        }
                        else
                        {
                            bat.logic.Helpers.AzureStorage.AzureBlobStorage.Download(bat.logic.Constants.Azure.AZURE_UPLOADED_IMAGES_STORAGE_CONTAINER, account.Picture).DownloadToStream(memoryStream);
                        }
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
    }
}
