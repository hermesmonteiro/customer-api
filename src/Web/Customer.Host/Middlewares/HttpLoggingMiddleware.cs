using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;

namespace Customer.Host.Middlewares
{
    public class HttpLoggingMiddleware
    {
        private readonly ILogger<HttpLoggingMiddleware> _logger;
        private readonly RequestDelegate _next;

        public HttpLoggingMiddleware(
            RequestDelegate next,
            ILogger<HttpLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            if (IsLoggingEnabled())
                await InvokeInternal(context);
            else
                await _next(context);
        }

        private bool IsLoggingEnabled()
        {
            return _logger.IsEnabled(LogLevel.Trace)
                || _logger.IsEnabled(LogLevel.Debug)
                || _logger.IsEnabled(LogLevel.Information);
        }

        private async Task InvokeInternal(HttpContext context)
        {
            await LogRequest(context);
            await LogResponse(context);
        }

        private async Task LogRequest(HttpContext context)
        {
            if (IsRequestLoggable(context))
            {
                var requestBody = await context.ReadRequestBody();

                _logger.LogInformation("Path: {0} \n Request: {1}", context.Request.Path.Value, requestBody);
            }
        }

        private async Task LogResponse(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;
            await using var responseBody = new RecyclableMemoryStreamManager().GetStream();
            context.Response.Body = responseBody;

            // Continue executing the remaining pipelines
            await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBodyAsString = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            if (IsRequestLoggable(context))
            {
                _logger.LogInformation("Response: {0}", responseBodyAsString);
            }

            await responseBody.CopyToAsync(originalBodyStream);
        }

        public bool IsRequestLoggable(HttpContext httpContext)
        {
            var path = httpContext.Request.Path.Value;
            return path.ToLower().IndexOf("health") == -1;
        }
    }
}
