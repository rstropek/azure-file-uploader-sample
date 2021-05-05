using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using AzureSqlEfcore.Data;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace AzureSqlEfcore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomersRepository repository;
        private readonly Uri blobContainerEndpoint;

        public CustomersController(ICustomersRepository repository, IConfiguration configuration)
        {
            this.repository = repository;
            blobContainerEndpoint = new Uri($"https://{configuration["Storage:AccountName"]}.blob.core.windows.net/{configuration["Storage:Container"]}");
        }

        /// <summary>
        /// Gets all customers from repository
        /// </summary>
        [HttpGet(Name = nameof(GetAllCustomers))]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomers()
            => Ok(await repository.GetAll());

        /// <summary>
        /// Gets a customer by id
        /// </summary>
        /// <param name="id">ID of the customer to find</param>
        [HttpGet("{id}", Name = nameof(GetCustomerById))]
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            var customer = await repository.TryGetById(id);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        /// <summary>
        /// Add a new customer
        /// </summary>
        /// <param name="customer">Data of the new customer</param>
        [HttpPost(Name = nameof(AddCustomer))]
        public async Task<ActionResult<Customer>> AddCustomer([FromBody] Customer customer)
        {
            var newCustomer = await repository.Add(customer);
            return CreatedAtRoute(nameof(GetCustomerById), new { id = newCustomer.ID }, newCustomer);
        }

        /// <summary>
        /// Update an existing customer
        /// </summary>
        /// <param name="id">ID of the customer to update</param>
        /// <param name="patch">Fields to update</param>
        [HttpPatch("{id}", Name = nameof(PatchCustomer))]
        public async Task<ActionResult<Customer>> PatchCustomer(int id, [FromBody] CustomerPatch patch)
        {
            var customer = await repository.TryPatch(id, patch);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        /// <summary>
        /// Delete an existing customer
        /// </summary>
        /// <param name="id">ID of the customer to delete</param>
        [HttpDelete("{id}", Name = nameof(DeleteCustomerById))]
        public async Task<ActionResult> DeleteCustomerById(int id)
        {
            if (!await repository.TryDeleteById(id)) return NotFound();
            return NoContent();
        }

        [HttpPost("import", Name = nameof(Import))]
        public async Task<ActionResult> Import([FromQuery] string sourceFile)
        {
            if (string.IsNullOrEmpty(sourceFile)) return BadRequest($"{nameof(sourceFile)} query parameter mising");

            // Download blob to temporary file. This is necessary if you have to handle large files
            // that might not fit into memory.
            // Tip: In practice, put blob handling in separate service to keep code clean. Not done here
            //      to keep things simple. Could be a nice exercise for you to practice.
            var containerClient = new BlobContainerClient(blobContainerEndpoint, new DefaultAzureCredential());
            var blobClient = containerClient.GetBlobClient(sourceFile);
            var tempFileName = Path.Combine(Environment.GetEnvironmentVariable("TMP")!, Guid.NewGuid().ToString());
            await blobClient.DownloadToAsync(tempFileName);

            try
            {
                // Read CSV file line-by-line (for large CSV files)
                using var reader = new StreamReader(tempFileName);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                if (!await csv.ReadAsync()) return BadRequest("Error while reading CSV");
                csv.ReadHeader();
                while (await csv.ReadAsync())
                {
                    var c = csv.GetRecord<CustomerStaging>();

                    // Note that adding database records line-by-line is very slow. In practice,
                    // consider calling `SaveChangesAsync` not after every add operation. *Bulk Insert*
                    // would even be better. Optimizing DB access is out-of-scope for this training.
                    await repository.Add(c);
                }

                await repository.MergeStaging();
            }
            finally
            {
                try { System.IO.File.Delete(tempFileName); } catch { }
            }

            return Ok();
        }
    }
}
