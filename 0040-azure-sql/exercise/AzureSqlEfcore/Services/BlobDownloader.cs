using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureSqlEfcore.Services
{
    public interface IBlobDownloader
    {
        Task<string> DownloadBlob(string blobName);
    }

    public class BlobDownloader : IBlobDownloader
    {
        private readonly Uri blobContainerEndpoint;

        public BlobDownloader(IConfiguration configuration)
        {
            blobContainerEndpoint = new Uri($"https://{configuration["Storage:AccountName"]}.blob.core.windows.net/{configuration["Storage:Container"]}");
        }

        public async Task<string> DownloadBlob(string blobName)
        {
            var containerClient = new BlobContainerClient(blobContainerEndpoint, new DefaultAzureCredential());
            var blobClient = containerClient.GetBlobClient(blobName);
            var tempFileName = Path.Combine(Environment.GetEnvironmentVariable("TMP")!, Guid.NewGuid().ToString());
            await blobClient.DownloadToAsync(tempFileName);
            return tempFileName;
        }
    }
}
