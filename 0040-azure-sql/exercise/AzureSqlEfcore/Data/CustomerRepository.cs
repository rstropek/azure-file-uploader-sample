using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureSqlEfcore.Data
{
    public class CustomerPatch
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Gender { get; set; }

        public string? IpAddress { get; set; }
    }

    public interface ICustomersRepository
    {
        Task<IEnumerable<Customer>> GetAll();

        Task<Customer?> TryGetById(int id);

        Task<Customer> Add(Customer c);

        Task<CustomerStaging> Add(CustomerStaging c);

        Task MergeStaging();

        Task<Customer?> TryPatch(int id, CustomerPatch patch);

        Task<bool> TryDeleteById(int id);
    }

    public class CustomersRepository : ICustomersRepository
    {
        private readonly ILogger<CustomersRepository> logger;
        private readonly DataContext dc;

        public CustomersRepository(ILogger<CustomersRepository> logger, DataContext dc)
        {
            this.logger = logger;
            this.dc = dc;
        }

        public async Task<IEnumerable<Customer>> GetAll() => await dc.Customers.ToArrayAsync();

        public async Task<Customer?> TryGetById(int id)
        {
            var c = await dc.Customers.FirstOrDefaultAsync(c => c.ID == id);
            if (c == null) logger.LogWarning("Search for customer with ID {id} failed.", id);
            return c;
        }

        public async Task<Customer> Add(Customer c)
        {
            dc.Customers.Add(c);
            await dc.SaveChangesAsync();
            logger.LogTrace("Added new customer {id}.", c.ID);
            return c;
        }

        public async Task<CustomerStaging> Add(CustomerStaging c)
        {
            dc.CustomersStaging.Add(c);
            await dc.SaveChangesAsync();
            logger.LogTrace("Added new customer {id}  to staging.", c.ID);
            return c;
        }

        public async Task MergeStaging()
        {
            await dc.Database.ExecuteSqlRawAsync(@"
                MERGE Customers AS target
                USING (SELECT FirstName, LastName, Email, Gender, IpAddress FROM CustomersStaging)
                    AS source(FirstName, LastName, Email, Gender, IpAddress)
                ON (source.Email = target.Email)
                WHEN MATCHED AND target.FirstName <> source.FirstName
                    OR target.LastName <> source.LastName 
                    OR target.Gender <> source.Gender 
                    OR target.IpAddress <> source.IpAddress THEN
                    UPDATE SET target.FirstName = source.FirstName,
                        target.LastName = source.LastName,
                        target.Gender = source.Gender,
                        target.IpAddress = source.IpAddress
                WHEN NOT MATCHED BY TARGET THEN
                    INSERT (FirstName, LastName, Email, Gender, IpAddress)
                    VALUES (FirstName, LastName, Email, Gender, IpAddress)
                WHEN NOT MATCHED BY SOURCE THEN
                    DELETE
                OUTPUT $action;");
            await dc.Database.ExecuteSqlRawAsync("TRUNCATE TABLE CustomersStaging");
        }

        public async Task<Customer?> TryPatch(int id, CustomerPatch patch)
        {
            var c = await dc.Customers.FirstOrDefaultAsync(c => c.ID == id);
            if (c == null)
            {
                logger.LogWarning("Patching of customer with ID {id} failed because ID was not found.", id);
                return null;
            }

            if (patch.FirstName != null) c.FirstName = patch.FirstName;
            if (patch.LastName != null) c.LastName = patch.LastName;
            if (patch.Gender != null) c.Gender = patch.Gender;
            if (patch.IpAddress != null) c.IpAddress = patch.IpAddress;

            await dc.SaveChangesAsync();
            return c;
        }

        public async Task<bool> TryDeleteById(int id)
        {
            var c = await dc.Customers.FirstOrDefaultAsync(c => c.ID == id);
            if (c == null)
            {
                logger.LogWarning("Removing customer with ID {id} failed because ID was not found.", id);
                return false;
            }

            dc.Customers.Remove(c);
            await dc.SaveChangesAsync();

            return true;
        }
    }
}
