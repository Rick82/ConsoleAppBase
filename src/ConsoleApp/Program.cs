using ConsoleApp2.Services;
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

        public class Worker : IHostedService
        {
            private readonly IMyService _myService;
            private readonly string _configKey;
            private readonly ILogger<Worker> _logger;
            public Worker(IMyService service, IConfiguration configuration, ILogger<Worker> logger, IOptions<SetupOptions> options)
            {
                _myService = service ?? throw new ArgumentNullException(nameof(service));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _configKey = configuration?["ConfigKey"] ??
                             throw new ArgumentNullException(nameof(configuration));
            }

            public async Task StartAsync(CancellationToken cancellationToken)
            {
                _logger.LogInformation("Read {key} from settings", _configKey);
                await _myService.PerformLongTaskAsync();
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}