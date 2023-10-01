using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Customer.Host
{
    public class Program
    {
        public static void Main(string[] args) =>
            CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                     .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                     .ConfigureLogging((context, builder) => builder.AddJsonConsole(
                         options =>
                         {
                             options.IncludeScopes = true;
#if DEBUG
                             options.JsonWriterOptions = new JsonWriterOptions
                             {
                                 Indented = true
                             };
#endif
                         }))
                     .UseMetricsWebTracking()
                     .UseConsoleLifetime();
    }
}
