using Azure.Storage.Blobs;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace CsvUploader
{
    internal static partial class CsvUploaderCommands
    {
        public static async Task List(ConnectionParameters parameters)
        {
            try
            {
                var container = new BlobContainerClient(BuildConnectionString(parameters), parameters.ContainerName);
                if (!await container.ExistsAsync())
                {
                    Log.Error("Container {containerName} does not exist, cannot list blobs", new { parameters.ContainerName });
                    return;
                }

                var names = new List<string>();
                await foreach (var blob in container.GetBlobsAsync())
                {
                    names.Add(blob.Name);
                }

                Console.WriteLine(JsonSerializer.Serialize(names));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while accessing blob storage");
            }
        }
    }
}
