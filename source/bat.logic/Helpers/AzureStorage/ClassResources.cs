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
    public class ClassResources
    {
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
