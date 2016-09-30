using bat.logic.Helpers.AzureStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace bat.logic.Helpers.AzureStorage
{
    public class StoredResources
    {
        public static string UploadLessonResource(HttpPostedFileBase file)
        {
            return Upload(file, Constants.Azure.AZURE_UPLOADED_LESSON_RESOURCES_STORAGE_CONTAINER);
        }

        public static void DownloadLessonResource(MemoryStream ms, string storageName)
        {
            AzureBlobStorage.Download(bat.logic.Constants.Azure.AZURE_UPLOADED_LESSON_RESOURCES_STORAGE_CONTAINER, storageName).DownloadToStream(ms);
        }

        public static string UploadProfilePicture(HttpPostedFileBase file)
        {
            return Upload(file, Constants.Azure.AZURE_UPLOADED_PROFILE_IMAGES_STORAGE_CONTAINER);
        }

        public static void DownloadProfilePicture(MemoryStream ms, string storageName)
        {
            AzureBlobStorage.Download(bat.logic.Constants.Azure.AZURE_UPLOADED_PROFILE_IMAGES_STORAGE_CONTAINER, storageName).DownloadToStream(ms);
        }
        public static string UploadExamPaper(HttpPostedFileBase file)
        {
            return Upload(file, Constants.Azure.AZURE_UPLOADED_EXAM_PAPERS_STORAGE_CONTAINER);
        }

        public static void DownloadExamPaper(MemoryStream ms, string storageName)
        {
            AzureBlobStorage.Download(bat.logic.Constants.Azure.AZURE_UPLOADED_EXAM_PAPERS_STORAGE_CONTAINER, storageName).DownloadToStream(ms);
        }

        public static string UploadLessonResourceImage(string data)
        {
            var storageName = Guid.NewGuid().ToString().Replace("{", "").Replace("}", "");
            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(data);
            //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
            MemoryStream stream = new MemoryStream(byteArray);
            AzureBlobStorage.Upload(stream, Constants.Azure.AZURE_UPLOADED_LESSON_RESOURCES_STORAGE_CONTAINER, storageName);

            return storageName;
        }

        private static string Upload(HttpPostedFileBase file, string container)
        {
            if (file == null) return null;

            if (file.ContentLength > 2500000)
                throw new Exception("Files can't exceed 2.5MB in size.");

            var ext = System.IO.Path.GetExtension(file.FileName);
            if (!logic.Rules.ImageValidation.ValidateExtension(ext))
                throw new Exception("Invalid file extension.");

            var storageName = Guid.NewGuid().ToString().Replace("{", "").Replace("}", "");

            AzureBlobStorage.Upload(file.InputStream, container, storageName);

            return storageName;
        }
    } 
}
