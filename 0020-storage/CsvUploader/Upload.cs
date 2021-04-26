using Azure.Storage.Blobs;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CsvUploader
{
    internal static partial class CsvUploaderCommands
    {
        public static async Task Upload(UploadParameters parameters)
        {
            try
            {
                var container = new BlobContainerClient(BuildConnectionString(parameters), parameters.ContainerName);

                var blob = container.GetBlobClient(parameters.DestinationFile);
                if (await blob.ExistsAsync())
                {
                    Log.Warning($"Destination file {parameters.DestinationFile} already exists");
                }

                await blob.UploadAsync(parameters.SourceFile, true);

                if (!string.IsNullOrEmpty(parameters.Customer))
                {
                    await blob.SetMetadataAsync(new Dictionary<string, string>() { { "Customer", parameters.Customer } });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while accessing blob storage");
            }
        }
    }
}
