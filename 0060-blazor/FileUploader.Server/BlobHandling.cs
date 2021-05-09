using System;
using System.Net;
using System.Threading.Tasks;
using Azure.Core.Serialization;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using FileUploader.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;

namespace FileUploader.Server
{
    public class BlobHandling
    {
        private static readonly string StorageAccountName;
        private static readonly Uri StorageUri;
        private static readonly string Container;
        private readonly IConfiguration Configuration;
        private readonly JsonObjectSerializer JsonObjectSerializer;
        private UserDelegationKey? UserDelegationKey;

        static BlobHandling()
        {
            StorageAccountName = Environment.GetEnvironmentVariable("StorageAccountName")!;
            StorageUri = new Uri($"https://{StorageAccountName}.blob.core.windows.net");
            Container = Environment.GetEnvironmentVariable("StorageContainer")!;
        }

        public BlobHandling(IConfiguration configuration, JsonObjectSerializer jsonObjectSerializer)
        {
            Configuration = configuration;
            JsonObjectSerializer = jsonObjectSerializer;
        }

        [Function(nameof(GetUploadSas))]
        public async Task<HttpResponseData> GetUploadSas(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            var fileName = $"{Guid.NewGuid()}.dat";

            if (UserDelegationKey == null || UserDelegationKey.SignedExpiresOn > DateTimeOffset.UtcNow.AddMinutes(-1))
            {
                var serviceClient = new BlobServiceClient(StorageUri, new DefaultAzureCredential());
                UserDelegationKey = await serviceClient.GetUserDelegationKeyAsync(
                    DateTime.UtcNow.AddMinutes(-1),
                    DateTime.UtcNow.AddDays(7));
            }

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = Container,
                BlobName = fileName,
                Resource = "b",
                StartsOn = DateTime.UtcNow.AddMinutes(-2),
                ExpiresOn = DateTime.UtcNow.AddMinutes(5),
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Create | BlobSasPermissions.Write | BlobSasPermissions.Add);

            var sas = sasBuilder.ToSasQueryParameters(UserDelegationKey, StorageAccountName);
            var blobUriBuilder = new BlobUriBuilder(new Uri(StorageUri, new PathString($"/{Container}").Add($"/{fileName}")))
            {
                Sas = sas
            };

            var response = req.CreateResponse();
            await response.WriteAsJsonAsync(
                new GetUploadSasResultDto(blobUriBuilder.ToString()), JsonObjectSerializer);
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }
    }
}
