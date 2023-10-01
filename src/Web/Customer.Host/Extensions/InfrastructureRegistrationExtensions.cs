using System.Text.Json.Serialization;
using Customer.Api;
using Customer.Infrastructure.Repositories.Customer;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OpenTelemetry.Trace;

namespace Customer.Host.Extensions
{
    public static class InfrastructureRegistrationExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
                                                           IConfiguration configuration)
        {
            services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });
            services.AddMetrics();

            services.AddControllers()
                    .AddJsonOptions(options =>
                     {
                         options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                         options.JsonSerializerOptions.IgnoreNullValues = true;
                     })
                    // .AddApplicationPart(typeof().Assembly)
                    .AddFluentValidation(c =>
                     {
                         c.RegisterValidatorsFromAssemblyContaining<ApiControllerBase>();
                         c.ValidatorFactoryType = typeof(HttpContextServiceProviderValidatorFactory);
                     });

            services.AddFeatureManagement();
            
            //services.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration =
            //        configuration.GetConnectionString("RedisCache");
            //});

            services.AddOpenTelemetryTracing(builder => builder
                                                       .AddAspNetCoreInstrumentation(options => options.Filter =
                                                            httpContext => httpContext.Request.Method
                                                               .Equals("GET"))
                                                       .AddJaegerExporter());


            // Sql Repositories
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<CustomerRepository>();

            return services;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app,
                                                            IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("CORSPolicy");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            return app;
        }
    }
}