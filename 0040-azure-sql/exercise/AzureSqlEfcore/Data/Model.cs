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
}
