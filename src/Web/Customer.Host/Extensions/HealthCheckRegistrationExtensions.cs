using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Customer.Host.Extensions
{
    public class HealthCheckResponse
    {
        public string Status { get; set; }
        public IEnumerable<HealthCheck> Checks { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public class HealthCheck
    {
        public string Status { get; set; }
        public string Component { get; set; }
        public string Description { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public static class HealthCheckRegistrationExtensions
    {
        public static IServiceCollection AddHealthChecksConfigurations(this IServiceCollection services,
                                                                       IConfiguration configuration)
        {
            services.AddHealthChecks();

            return services;
        }

        public static IApplicationBuilder UseHealthCheckConfigurations(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/health",
                                new HealthCheckOptions
                                {
                                    ResponseWriter = async (context,
                                                            report) =>
                                    {
                                        context.Response.ContentType = "application/json";

                                        var response = new HealthCheckResponse
                                        {
                                            Status = report.Status.ToString(),
                                            Checks = report.Entries.Select(x => new HealthCheck
                                            {
                                                Component = x.Key,
                                                Status = x.Value.Status.ToString(),
                                                Description = x.Value.Description,
                                                Duration = x.Value.Duration
                                            }),
                                            Duration = report.TotalDuration
                                        };
                                        await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                                    }
                                });

            return app;
        }
    }
}