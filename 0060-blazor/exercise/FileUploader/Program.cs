using FileUploader.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FileUploader
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            // Typed HTTP Client. See also
            // https://docs.microsoft.com/en-us/aspnet/core/blazor/call-web-api?view=aspnetcore-5.0#typed-httpclient
            builder.Services.AddHttpClient<FileUploaderHttpClient>(client =>
                client.BaseAddress = new Uri(builder.Configuration["WebApiBase"]));

            await builder.Build().RunAsync();
        }
    }
}
