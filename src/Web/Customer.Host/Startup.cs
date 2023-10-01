using Customer.Domain.Configurations;
using Customer.Host.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Customer.Host
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) =>
            services.AddServices(Configuration)
                .AddInfrastructure(Configuration)
                .AddVersioningConfigurations()
                .AddOptions(Configuration, connectionStrings => connectionStrings
                                            .SetProperties(Configuration.GetConnectionString(DbConnectionStrings.LaunchDbConnectionStringName)))
                .AddHealthChecksConfigurations(Configuration);

        public void Configure(IApplicationBuilder app,
                              IWebHostEnvironment env) =>
            app.UseMiddleware(env)
               .UseInfrastructure(env)
               .UseSwaggerConfigurations(env)
               .UseHealthCheckConfigurations()
               .UseCorsConfigurations();
    }
}