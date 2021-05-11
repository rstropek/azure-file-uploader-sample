using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(FileUploaders.Functions.Startup))]

namespace FileUploaders.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddTransient<ICustomerBulkInserter, CustomerBulkInserter>();
            builder.Services.AddSingleton<IAuthorize, Authorize>();
        }
    }
}
