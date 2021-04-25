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
        public static Task Upload(UploadParameters parameters)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while accessing blob storage");
            }

            return Task.CompletedTask;
        }
    }
}
