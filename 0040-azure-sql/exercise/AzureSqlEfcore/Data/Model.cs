using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;

namespace AzureSqlEfcore.Data
{
    public class Customer
    {
        public int ID { get; set; }

        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        [MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Gender { get; set; }

        [MaxLength(15)]
        public string? IpAddress { get; set; }
    }

    public class CustomerStaging
    {
        [Name("id")]
        public int ID { get; set; }

        [Name("first_name")]
        [MaxLength(100)]
        public string? FirstName { get; set; }

        [Name("last_name")]
        [MaxLength(100)]
        public string? LastName { get; set; }

        [Name("email")]
        [MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Name("gender")]
        [MaxLength(50)]
        public string? Gender { get; set; }

        [Name("ip_address")]
        [MaxLength(15)]
        public string? IpAddress { get; set; }
    }
}
