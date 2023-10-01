using Customer.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Customer.Host.Extensions
{
    public static class ServicesRegistrationExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services,
                                                     IConfiguration configuration)
        {
            //services.AddScoped<IGetLaunchService, GetLaunchService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(new HealthCheckCircuitBreakerService()); services.AddSwaggerGen();

            return services;
        }
    }
}