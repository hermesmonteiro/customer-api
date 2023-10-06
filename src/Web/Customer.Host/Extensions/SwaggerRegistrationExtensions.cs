using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Customer.Host.Extensions
{
    public static class SwaggerRegistrationExtensions
    {
        public static IServiceCollection AddSwaggerConfigurations(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(type => type.ToString());

                c.SwaggerDoc("v1",
                             new OpenApiInfo
                             {
                                 Version = "v1",
                                 Title = "Launch API",
                                 Description = "Launch Web API",
                                 TermsOfService = new Uri("https://example.com/terms"),
                                 Contact = new OpenApiContact
                                 {
                                     Name = "Shayne Boyer",
                                     Email = string.Empty,
                                     Url = new Uri("https://twitter.com/spboyer")
                                 },
                                 License = new OpenApiLicense
                                 {
                                     Name = "Use under LICX",
                                     Url = new Uri("https://example.com/license")
                                 }
                             });

                c.TagActionsBy(api =>
                {
                    if (api.GroupName != null)
                        return new[]
                               {
                                   api.GroupName
                               };

                    var controllerActionDescriptor = api.ActionDescriptor as ControllerActionDescriptor;

                    if (controllerActionDescriptor != null)
                        return new[]
                               {
                                   controllerActionDescriptor.ControllerName
                               };

                    throw new InvalidOperationException("Unable to determine tag for endpoint.");
                });

                c.DocInclusionPredicate((name,
                                         api) => true);
                c.EnableAnnotations();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerConfigurations(this IApplicationBuilder app,
                                                                   IWebHostEnvironment env)
        {
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                {                   
                });
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("./swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            return app;
        }
    }
}