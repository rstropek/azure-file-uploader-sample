using Azure.Storage;
using Azure.Storage.Sas;
using Serilog;
using System;

namespace CsvUploader
{
    internal static partial class CsvUploaderCommands
    {
        public static StorageSharedKeyCredential GetCredential(ConnectionParameters parameters)
        {
            if (parameters.UseAzurite) return new(AzuriteAccountName, AzuriteAccountKey);
            else
            {
                parameters.EnsureNameAndKeyAreSet();
                return new(parameters.AccountName, parameters.AccountKey);
            }
        }

        public static void Share(ShareParameters parameters)
        {
            try
            {
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = parameters.ContainerName,
                    BlobName = parameters.File,
                    Resource = "b",
                    StartsOn = DateTime.UtcNow.AddMinutes(-2),
                    ExpiresOn = DateTime.UtcNow.AddHours(parameters.Hours),
                };
                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                var sas = sasBuilder.ToSasQueryParameters(GetCredential(parameters));
                Console.WriteLine(sas);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while accessing blob storage");
            }
        }
    }
}
