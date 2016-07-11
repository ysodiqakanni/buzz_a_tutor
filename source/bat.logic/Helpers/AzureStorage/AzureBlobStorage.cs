using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Logic.Helpers.AzureStorage
{
    public class AzureBlobStorage
    {
        public static CloudBlockBlob Upload(Stream ms, string accContainer, string storageName)
        {
            if (string.IsNullOrEmpty(accContainer)) throw new Exception("Container name required.");
            if (string.IsNullOrEmpty(storageName)) throw new Exception("Storage name required.");

            //Storage Account
            var storageAccount = CloudStorageAccount.Parse(
                "DefaultEndpointsProtocol=https;AccountName=" + bat.logic.Constants.Azure.AZURE_STORAGE_ACCOUNT_NAME +
                ";AccountKey=" + bat.logic.Constants.Azure.AZURE_STORAGE_ACCOUNT_KEY);

            // Create the blob client
            var blobClient = storageAccount.CreateCloudBlobClient();

            //Retrieve a reference to a container
            var container = blobClient.GetContainerReference(accContainer);

            //Retrieve reference to a blob using Param 'storageName'
            var blockBlob = container.GetBlockBlobReference(storageName);

            //Create or overwrite the 'storageName' blob with the contents of ms.
            blockBlob.UploadFromStream(ms);
            return blockBlob;
        }

        public static CloudBlockBlob Download(string accContainer, string storageName)
        {
            if (string.IsNullOrEmpty(accContainer)) throw new Exception("Container name required.");
            if (string.IsNullOrEmpty(storageName)) throw new Exception("Storage name required.");

            //Storage Account
            var storageAccount = CloudStorageAccount.Parse(
                "DefaultEndpointsProtocol=https;AccountName=" + bat.logic.Constants.Azure.AZURE_STORAGE_ACCOUNT_NAME +
                ";AccountKey=" + bat.logic.Constants.Azure.AZURE_STORAGE_ACCOUNT_KEY);

            //Create the blob client
            var blobClient = storageAccount.CreateCloudBlobClient();

            //Retrieve reference to a previously create container
            var container = blobClient.GetContainerReference(accContainer);

            //Returns the reference to the blob.
            return container.GetBlockBlobReference(storageName);
        }

        public static void Delete(string accContainer, string storageName)
        {
            if (string.IsNullOrEmpty(accContainer)) throw new Exception("Container name required.");
            if (string.IsNullOrEmpty(storageName)) throw new Exception("Storage name required.");

            //Storage account
            var storageAccount = CloudStorageAccount.Parse(
                "DefaultEndpointsProtocol=https;AccountName=" + bat.logic.Constants.Azure.AZURE_STORAGE_ACCOUNT_NAME +
                ";AccountKey=" + bat.logic.Constants.Azure.AZURE_STORAGE_ACCOUNT_KEY);

            //Create the blob client
            var blobClient = storageAccount.CreateCloudBlobClient();

            //Retrieve reference to a previously created container.
            var container = blobClient.GetContainerReference(accContainer);
            
            //Retrieve reference to a blob 
            var blockBlob = container.GetBlockBlobReference(storageName);

            // Deletes the blob
            blockBlob.DeleteIfExists();
        }

        public static void Copy(string accContainer, string source, string target)
        {
            if (string.IsNullOrEmpty(accContainer)) throw new Exception("Container name required.");
            if (string.IsNullOrEmpty(source)) throw new Exception("Source required.");
            if (string.IsNullOrEmpty(target)) throw new Exception("Target required.");

            var storageAccount = CloudStorageAccount.Parse(
                "DefaultEndpointsProtocol=https;AccountName=" + bat.logic.Constants.Azure.AZURE_STORAGE_ACCOUNT_NAME +
                ";AccountKey=" + bat.logic.Constants.Azure.AZURE_STORAGE_ACCOUNT_KEY);

            var cloudBlobClient = storageAccount.CreateCloudBlobClient();
            var sourceContainer = cloudBlobClient.GetContainerReference(accContainer);
            var targetContainer = cloudBlobClient.GetContainerReference(accContainer);
            var sourceBlob = sourceContainer.GetBlockBlobReference(source);
            var targetBlob = targetContainer.GetBlockBlobReference(target);

            targetBlob.StartCopy(sourceBlob);
        }
    }
}
