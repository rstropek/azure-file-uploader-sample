using Azure.Core.Serialization;
using FileUploader.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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

            builder.Services.AddScoped<GraphAPIAuthorizationMessageHandler>();
            builder.Services.AddScoped<FileUploaderAuthorizationMessageHandler>();

            builder.Services.AddHttpClient("GraphAPI",
                    client => client.BaseAddress = new Uri("https://graph.microsoft.com"))
                .AddHttpMessageHandler<GraphAPIAuthorizationMessageHandler>();
            builder.Services.AddHttpClient<FileUploaderHttpClient>(client =>
                client.BaseAddress = new Uri(builder.Configuration["WebApiBase"]))
                .AddHttpMessageHandler<FileUploaderAuthorizationMessageHandler>();

            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
                options.ProviderOptions.DefaultAccessTokenScopes.Add("User.Read");
                options.ProviderOptions.DefaultAccessTokenScopes.Add("app://b89440c8-04f2-43c5-a6e7-104b9b31b77c/Read");
                options.ProviderOptions.DefaultAccessTokenScopes.Add("app://b89440c8-04f2-43c5-a6e7-104b9b31b77c/Write");
            });

            await builder.Build().RunAsync();
        }
    }
}
