using System;
using Customer.Api.Configurations;
using Customer.Domain.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Customer.Host.Extensions
{
    public static class OptionsRegistrationExtensions
    {
        public static IServiceCollection AddOptions(this IServiceCollection services,
                                                    IConfiguration configuration,
                                                    Action<DbConnectionStrings> connectionString)
        {
            services.Configure<AuthTokenOptions>(configuration.GetSection("Auth"));
            services.Configure<ApiCasinoExternalSettings>(configuration.GetSection("ApiCasinoExternalSettings"));

            services
                .AddOptions<DbConnectionStrings>()
                .Configure(connectionString)
                .ValidateDataAnnotations();

            return services;
        }
    }
}