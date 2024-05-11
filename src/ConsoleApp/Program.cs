using ConsoleApp2.Services;
using ConsoleAppBase;
using ConsoleAppBase.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Core;
namespace ConsoleApp2
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            var host = CreateHostBuilder();
            await host.RunConsoleAsync();
            return Environment.ExitCode;
        }

        private static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostContext, builder) =>
                {
                    var env = hostContext.HostingEnvironment;
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    builder.AddJsonFile($"appsettings{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    builder.AddEnvironmentVariables();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                })
                .UseSerilog((hostContext, loggerConfiguration) =>
                {
                    loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
                })
                .ConfigureServices((hostBuilderContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddTransient<IMyService, MyService>();
                    services.AddOptions<SetupOptions>()
                        .Bind(hostBuilderContext.Configuration.GetSection(SetupOptions.ConfigKey));
                    
                });
        }
    }
}