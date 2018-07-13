using System;
using System.IO;
using System.Threading.Tasks;
using Helpers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Xunit;

namespace ASAManager
{
    public static class Helpers
    {
        private const int BLOB_INTERVAL_MSEC = 60000;
        /**
         * Check that a file exists with the given filename for the
         * given blob container. Return true if it exists, false otherwise.
         */
        public static async Task<bool> CheckBlobFileExists(
            string blobStorageConnectionString,
            string containerName,
            string fileName)
        {
            BlobContinuationToken token = null;
            CloudBlobContainer container = GetBlobContainer(
                blobStorageConnectionString,
                containerName);
            do
            {
                var results = await container.ListBlobsSegmentedAsync(
                    null,
                    true,
                    BlobListingDetails.All,
                    null,
                    token,
                    null,
                    null);
                token = results.ContinuationToken;
                foreach (var item in results.Results)
                {
                    if (item.Uri.AbsolutePath.Contains(fileName))
                    {
                        return true;
                    }
                }
            } while (token != null);

            return false;
        }

        /**
         * Get the blob storage connection string by querying
         * environment variables.
         */
        public static string GetStorageConnectionString()
        {
            string blobStorageAccount = Environment.GetEnvironmentVariable(Constants.AZUREBLOB_ACCOUNT_ENV_VAR);
            string blobStorageKey = Environment.GetEnvironmentVariable(Constants.AZUREBLOB_KEY_ENV_VAR);
            string blobStorageEndpoint = Environment.GetEnvironmentVariable(Constants.AZUREBLOB_ENDPOINT_ENV_VAR);
            return $"DefaultEndpointsProtocol=https;AccountName={blobStorageAccount};AccountKey={blobStorageKey};EndpointSuffix={blobStorageEndpoint}";
        }

        /**
         * Returns true if the given blob has a file with the
         * given filename between start and now, and that the
         * file contains the given search string.
         */
        public static bool CheckIfBlobExistsAndContainsString(
            DateTime start,
            string blobStorageConnectionString,
            string containerName,
            string fileName,
            string searchString)
        {
            return CheckIfBlobExistsAndIsValid(
                start,
                blobStorageConnectionString,
                containerName,
                fileName,
                searchString,
                true);
        }

        /**
         * Returns true if the given blob has a file with the
         * given filename between start and now, and that the
         * file does not contain the given search string.
         */
        public static bool CheckIfBlobExistsAndDoesNotContainString(
            DateTime start,
            string blobStorageConnectionString,
            string containerName,
            string fileName,
            string searchString)
        {
            return CheckIfBlobExistsAndIsValid(
                start,
                blobStorageConnectionString,
                containerName,
                fileName,
                searchString,
                false);
        }

        /**
         * Checks if a new file with given file name was
         * written to blob storage between the given start time and the current time.
         * If a file is found, check if file contains given search string. If
         * expectToContainSearchString is true, return true if contains searchString,
         * false otherwise. If expectToContainSearchString is false, return false if contains
         * searchString, true otherwise.
         */
        private static bool CheckIfBlobExistsAndIsValid(
            DateTime start,
            string blobStorageConnectionString,
            string containerName,
            string fileName,
            string searchString,
            bool expectToContainSearchString)
        {
            DateTime end = DateTime.UtcNow;

            CloudBlobContainer container = GetBlobContainer(
                blobStorageConnectionString,
                containerName);
            DateTime checkTime = start;
            while (checkTime.Subtract(end).TotalMilliseconds < BLOB_INTERVAL_MSEC)
            {
                string date = checkTime.ToString("yyyy-MM-dd");
                string time = checkTime.ToString("HH-mm");
                string blobName = $"{date}/{time}/{fileName}";
                CloudBlob blob = container.GetBlobReference(blobName);
                bool exists = blob.ExistsAsync().Result;
                if (exists && CheckBlobForString(searchString, blob, expectToContainSearchString).Result)
                {
                    return true;
                }

                checkTime = checkTime.AddMinutes(1);
            }
            return false;
        }

        /**
         * Check if given blob contains given search string. If
         * expectToContainSearchString is true, return true if contains searchString,
         * false otherwise. If expectToContainSearchString is false, return false if contains
         * searchString, true otherwise.
         */
        private static async Task<bool> CheckBlobForString(
            string searchString,
            CloudBlob blob,
            bool expectToContainSearchString)
        {
            string tempFile = Path.GetTempFileName();
            await blob.DownloadToFileAsync(tempFile, FileMode.Open);
            string line = "";
            using (var reader = new StreamReader(tempFile))
            {
                if (!reader.EndOfStream)
                {
                    line = reader.ReadToEnd();
                }
            }

            File.Delete(tempFile);
            if (expectToContainSearchString)
            {
                return line.Contains(searchString);
            }
            else
            {
                return !line.Contains(searchString);
            }
        }

        /**
         * Returns the reference data blob storage container
         */
        private static CloudBlobContainer GetBlobContainer(string blobStorageConnectionString, string containerName)
        {
            Assert.True(CloudStorageAccount.TryParse(blobStorageConnectionString, out var storageAccount));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(containerName);
        }
    }
}
