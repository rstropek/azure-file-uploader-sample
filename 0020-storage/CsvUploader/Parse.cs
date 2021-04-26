using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CsvUploader
{
    public class Customer
    {
        [Name("id")] public int Id { get; set; }
        [Name("first_name")] public string FirstName { get; set; } = string.Empty;
        [Name("last_name")] public string LastName { get; set; } = string.Empty;
        [Name("email")] public string Email { get; set; } = string.Empty;
        [Name("gender")] public string Gender { get; set; } = string.Empty;
        [Name("ip_address")] public string IpAddress { get; set; } = string.Empty;
    }

    internal static partial class CsvUploaderCommands
    {
        public static async Task Parse(ParseParameters parameters)
        {
            try
            {
                var container = new BlobContainerClient(BuildConnectionString(parameters), parameters.ContainerName);
                var blob = container.GetBlobClient(parameters.File);

                var leaseClient = blob.GetBlobLeaseClient();
                var lease = await leaseClient.AcquireAsync(TimeSpan.FromMinutes(1));
                try
                {
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        NewLine = "\n",
                        Delimiter = ",",
                    };

                    using var importStream = new MemoryStream();
                    await blob.DownloadToAsync(importStream);
                    importStream.Seek(0, SeekOrigin.Begin);

                    using var reader = new StreamReader(importStream);
                    using var csv = new CsvReader(reader, config);

                    var result = new List<Customer>();
                    await foreach (var record in csv.GetRecordsAsync<Customer>())
                    {
                        result.Add(record);
                    }

                    Console.WriteLine(JsonSerializer.Serialize(result.Take(3),
                        new JsonSerializerOptions { WriteIndented = true }));
                }
                finally
                {
                    await leaseClient.ReleaseAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while accessing blob storage");
            }
        }
    }
}
