using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace HelloAspNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobsController : ControllerBase
    {
        private readonly string BlobContainerEndpoint;

        public BlobsController(IConfiguration configuration)
        {
            BlobContainerEndpoint = configuration["BlobContainerEndpoint"];
        }

        [HttpGet(Name = nameof(GetHelloWorldBlob))]
        public async Task<ActionResult<string>> GetHelloWorldBlob()
        {
            var containerClient = new BlobContainerClient(new Uri(BlobContainerEndpoint), new DefaultAzureCredential());
            var blobClient = containerClient.GetBlockBlobClient("hello-world.txt");
            using var readStream = new MemoryStream();
            await blobClient.DownloadToAsync(readStream);
            return Ok(Encoding.UTF8.GetString(readStream.ToArray()));
        }
    }
}
