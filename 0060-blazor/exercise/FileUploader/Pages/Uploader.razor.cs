using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileUploader.Pages
{
    public partial class Uploader
    {
        /// <summary>
        /// Indicates whether a file is currently uploaded
        /// </summary>
        private bool IsUploading { get; set; } = false;

        /// <summary>
        /// Gets or sets currently selected file
        /// </summary>
        private IBrowserFile? CurrentFile { get; set; }

        /// <summary>
        /// Current status message
        /// </summary>
        private string? StatusMessage { get; set; }

        /// <summary>
        /// Indicates whether the component is in error state (i.e. <see cref="StatusMessage"/> is an error message)
        /// </summary>
        private bool IsInErrorStatus { get; set; }

        /// <summary>
        /// Helper to clear file selection
        /// </summary>
        private bool Toggle { get; set; }

        private void LoadFiles(InputFileChangeEventArgs e) => CurrentFile = e.GetMultipleFiles(1)[0];

        async Task OnUpload()
        {
            if (CurrentFile == null) return;

            try
            {
                IsUploading = true;
                StatusMessage = null;
                IsInErrorStatus = false;

                // Get blob uri and build blob client from it
                var getSasDto = await HttpClient.GetUploadSas();
                var blobClient = new BlobClient(new Uri(getSasDto.FileName));

                // Upload file
                await blobClient.UploadAsync(CurrentFile.OpenReadStream(), new BlobUploadOptions());
                await blobClient.SetMetadataAsync(new Dictionary<string, string>() {
                    { "originalname", CurrentFile.Name }
                });

                StatusMessage = "File successfully uploaded";
                Toggle = !Toggle;
                CurrentFile = null;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error while uploading file: {ex.Message}";
                IsInErrorStatus = true;
            }
            finally
            {
                IsUploading = false;
            }
        }
    }
}
