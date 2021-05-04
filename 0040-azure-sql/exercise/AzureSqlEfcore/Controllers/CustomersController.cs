using AzureSqlEfcore.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureSqlEfcore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomersRepository repository;

        public CustomersController(ICustomersRepository repository)
        {
            this.repository = repository;
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
    }
}
