using Logic.Helpers.AzureStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace bat.logic.Helpers.AzureStorage
{
    class UploadFile
    {
        /// <summary>
        /// Uploads the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// Images can't exceed 2.5MB in size.
        /// or
        /// Invalid file extension.
        /// </exception>
        public static string Upload(HttpPostedFileBase file)
        {
            if (file == null) return null;

            if (file.ContentLength > 2500000)
                throw new Exception("Files can't exceed 2.5MB in size.");

            var storageName = Guid.NewGuid().ToString().Replace("{", "").Replace("}", "");

            AzureBlobStorage.Upload(file.InputStream, Constants.Azure.AZURE_UPLOADED_IMAGES_STORAGE_CONTAINER, storageName);

            return storageName;
        }
    }
}
