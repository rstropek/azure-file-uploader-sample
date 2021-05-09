using Azure.Core.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace FileUploader.Server
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices((context, services) =>
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    };
                    services.AddSingleton(jsonOptions);
                    services.AddSingleton(_ => new JsonObjectSerializer(jsonOptions));
                })
                .Build();

            host.Run();
        }
    }
}