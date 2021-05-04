using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AzureSqlEfcore.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<CustomerStaging> CustomersStaging => Set<CustomerStaging>();

        private static readonly ILoggerFactory MyLoggerFactory
            = LoggerFactory.Create(builder => { builder.AddConsole(); });

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseLoggerFactory(MyLoggerFactory);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().HasIndex(c => c.Email).IsUnique();
            modelBuilder.Entity<CustomerStaging>().HasIndex(c => c.Email).IsUnique();
        }
    }
}
