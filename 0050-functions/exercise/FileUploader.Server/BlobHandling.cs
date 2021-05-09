using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Azure.Core.Serialization;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using FileUploader.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FileUploader.Server
{
    public class BlobHandling
    {
        private static readonly string StorageConnection;
        private static readonly Uri StorageConnectionUri;
        private static readonly StorageSharedKeyCredential StorageCredentials;
        private readonly IConfiguration Configuration;
        private readonly JsonObjectSerializer JsonObjectSerializer;
        private readonly ILogger<BlobHandling> Logger;

        private const string Container = "csv-upload";

        static BlobHandling()
        {
            StorageConnection = Environment.GetEnvironmentVariable("StorageConnection")!;

            var connStringArray = StorageConnection
                .Split(';')
                .Select(setting =>
                    new[]
                    {
                        setting[..setting.IndexOf('=')],
                        setting[(setting.IndexOf('=') + 1)..]
                    })
                .ToArray();
            var storageAccountName = connStringArray.First(s => s[0] == "AccountName")[1];
            StorageCredentials = new(storageAccountName, connStringArray.First(s => s[0] == "AccountKey")[1]);
            StorageConnectionUri = new Uri($"https://{storageAccountName}.blob.core.windows.net");
        }

        public BlobHandling(IConfiguration configuration, JsonObjectSerializer jsonObjectSerializer,
            ILogger<BlobHandling> logger)
        {
            Configuration = configuration;
            JsonObjectSerializer = jsonObjectSerializer;
            Logger = logger;
        }

        [Function(nameof(GetUploadSas))]
        public async Task<HttpResponseData> GetUploadSas(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            // Generate target file name
            var fileName = $"{Guid.NewGuid()}.dat";

            // Setup SAS builder
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = "csv-upload",
                BlobName = fileName,
                Resource = "b",
                StartsOn = DateTime.UtcNow.AddMinutes(-2),
                ExpiresOn = DateTime.UtcNow.AddMinutes(5),
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Create | BlobSasPermissions.Write | BlobSasPermissions.Add);

            // Generate SAS
            var sas = sasBuilder.ToSasQueryParameters(StorageCredentials);

            // Build target blob uri with SAS
            var targetBlobUri = new Uri(StorageConnectionUri, new PathString($"/{Container}").Add($"/{fileName}"));
            var blobUriBuilder = new BlobUriBuilder(targetBlobUri) { Sas = sas };

            // Return result
            var response = req.CreateResponse();
            await response.WriteAsJsonAsync(
                new GetUploadSasResultDto { FileName = blobUriBuilder.ToString() }, JsonObjectSerializer);
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }

        [Function(nameof(ProcessBlob))]
        public async Task ProcessBlob(
            [BlobTrigger("csv-upload/{name}", Connection = "StorageConnection")] string content, string name)
        {
            // Note problem: With .NET 5, it is NOT possible to get only a reference to the blob or a stream.
            // We always get the blob content -> not a good option for large blobs.

            Logger.LogInformation($"Process blob triggered with file {name}");
            Logger.LogInformation(content[..Math.Min(300, content.Length)]);

            // Todo: Implement processing of CSV. Here only simulated
            await Task.Delay(100);
        }
    }
}
