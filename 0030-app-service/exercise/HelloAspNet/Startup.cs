using HelloAspNet.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HelloAspNet
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Read more about dependency injection at
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
            services.AddSingleton<IHeroesRepository, HeroesRepository>();

            // Read more about Application Insights with ASP.NET Core at
            // https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core
            services.AddApplicationInsightsTelemetry();

            // Read more about creating API controller at
            // https://docs.microsoft.com/en-us/aspnet/core/web-api/
            services.AddControllers();

            // Read more about enabling CORS at
            // https://docs.microsoft.com/en-us/aspnet/core/security/cors
            services.AddCors();

            // Read more about NSwag at
            // https://github.com/RicoSuter/NSwag
            services.AddOpenApiDocument(doc =>
            {
                doc.Description = "This is a demo API.";
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/hello/world"))
                {
                    await context.Response.WriteAsync("Hello world!");
                    return;
                }

                await next();
            });

            app.Map("/hello/universe", universeApp =>
            {
                universeApp.Run(async context => await context.Response.WriteAsync("Hello universe!"));
            });
        }
    }
}
