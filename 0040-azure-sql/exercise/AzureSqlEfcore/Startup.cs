using AzureSqlEfcore.Data;
using AzureSqlEfcore.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AzureSqlEfcore
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
            services.AddDbContext<DataContext>(options => options.UseSqlServer(
                Configuration["ConnectionStrings:DefaultConnection"]));
            services.AddScoped<ICustomersRepository, CustomersRepository>();

            services.AddSingleton<IBlobDownloader, BlobDownloader>();

            // Read more about Application Insights with ASP.NET Core at
            // https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core
            services.AddApplicationInsightsTelemetry();

            // Read more about enabling CORS at
            // https://docs.microsoft.com/en-us/aspnet/core/security/cors
            services.AddCors();

            services.AddControllers();
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
        }
    }
}
