using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Azure.Storage.Sas;
using Azure.Storage.Blobs;
using FileUploader.Shared;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.ServiceBus;
using Azure.Messaging.ServiceBus;

namespace FileUploaders.Functions
{
    public class BlobHandling
    {
        private static readonly string StorageConnection;
        private static readonly Uri StorageConnectionUri;
        private static readonly StorageSharedKeyCredential StorageCredentials;
        private readonly IConfiguration Configuration;
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

        public BlobHandling(IConfiguration configuration, ILogger<BlobHandling> logger)
        {
            Configuration = configuration;
            Logger = logger;
        }

        [FunctionName(nameof(GetUploadSas))]
        public IActionResult GetUploadSas([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            var fileName = $"{Guid.NewGuid()}.dat";
            var sas = GetBlobSas(Container, fileName, BlobSasPermissions.Create | BlobSasPermissions.Write | BlobSasPermissions.Add);
            var targetBlobUri = new Uri(StorageConnectionUri, new PathString($"/{Container}").Add($"/{fileName}"));
            var blobUriBuilder = new BlobUriBuilder(targetBlobUri) { Sas = sas };
            return new OkObjectResult(new GetUploadSasResultDto { FileName = blobUriBuilder.ToString() });
        }

        private static BlobSasQueryParameters GetBlobSas(string containerName, string fileName, BlobSasPermissions permissions)
        {
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = fileName,
                Resource = "b",
                StartsOn = DateTime.UtcNow.AddMinutes(-2),
                ExpiresOn = DateTime.UtcNow.AddMinutes(5),
            };
            sasBuilder.SetPermissions(permissions);

            var sas = sasBuilder.ToSasQueryParameters(StorageCredentials);
            return sas;
        }

        [FunctionName(nameof(ProcessBlob))]
        public async Task ProcessBlob(
            [BlobTrigger(Container + "/{name}", Connection = "StorageConnection")] Stream blobStream, string name,
            [Blob(Container, Connection = "StorageConnection")] BlobContainerClient uploadContainer,
            [Blob("csv-processed", Connection = "StorageConnection")] BlobContainerClient processedContainer,
            [ServiceBus("importsuccess", Connection = "ServiceBusConnection", EntityType = EntityType.Topic)] IAsyncCollector<ServiceBusMessage> sucessMsg,
            [ServiceBus("importerror", Connection = "ServiceBusConnection", EntityType = EntityType.Topic)] IAsyncCollector<ServiceBusMessage> errorMsg)
        {
            Logger.LogInformation($"Processing file {name}");

            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ",", };
                using var reader = new StreamReader(blobStream);
                using var csv = new CsvReader(reader, config);

                var result = new List<Customer>();
                await foreach (var record in csv.GetRecordsAsync<Customer>())
                {
                    result.Add(record);
                }

                // Note: Here we would write the records to the DB. As this is EXACTLY the same
                // code as discussed in our previous example, the code is not duplicated here.
                // For demo purposes, we log the first three elements from the CSV.

                Logger.LogInformation(JsonSerializer.Serialize(result.Take(3), new JsonSerializerOptions { WriteIndented = true }));
            }
            catch (Exception)
            {
                await errorMsg.AddAsync(new ServiceBusMessage(name));
                return;
            }

            var sas = GetBlobSas(Container, name, BlobSasPermissions.Read | BlobSasPermissions.Delete);
            var sourceBlobUriBuilder = new UriBuilder(StorageConnectionUri)
            {
                Path = new PathString($"/{Container}").Add($"/{name}"),
                Query = sas.ToString()
            };

            var targetBlob = processedContainer.GetBlobClient(name);
            await targetBlob.SyncCopyFromUriAsync(sourceBlobUriBuilder.Uri);

            var sourceBlob = uploadContainer.GetBlobClient(name);
            await sourceBlob.DeleteAsync();

            await sucessMsg.AddAsync(new ServiceBusMessage(name));
        }

        [FunctionName(nameof(ProcessSuccess))]
        public void ProcessSuccess(
            [ServiceBusTrigger("importsuccess", "successlog", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage msg)
        {
            Logger.LogInformation($"File {msg.Body} successfully processed");
        }

        [FunctionName(nameof(ProcessError))]
        public void ProcessError(
            [ServiceBusTrigger("importerror", "errorlog", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage msg)
        {
            Logger.LogError($"Error while importing {msg.Body}");
        }
    }
}
