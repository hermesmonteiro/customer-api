using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Customer.Host.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private string _requestBody;

        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _webHostEnvironment;
        ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(
            RequestDelegate next,
            IWebHostEnvironment webHostEnvironment,
            ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                _requestBody = await context.ReadRequestBody();
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                var response = context.Response;
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.ContentType = "application/json";

                await response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails
                {
                    Status =
                        StatusCodes.Status500InternalServerError,
                    Type = "https://httpstatuses.com/500",
                    Title = ex.Message,
                    Detail =
                        _webHostEnvironment.EnvironmentName ==
                        "Development"
                            ? ex.StackTrace
                            : string.Empty,
                    Instance = response.HttpContext.Request.Path
                }));
            }
        }
    }
}