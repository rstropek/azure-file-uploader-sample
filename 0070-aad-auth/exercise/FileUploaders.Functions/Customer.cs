using CsvHelper.Configuration.Attributes;

namespace FileUploaders.Functions
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
}
