using Alphatag_Game.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Alphatag_Game
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSingleton<LiteDbToJsonService>(provider =>
                    {
                        string userName = Environment.UserName;
                        string dbFilePath = Path.Combine(@"C:\Users", userName, "AppData", "Local", "Laserwar", "alphatag", "alphatag.db");
                        string outputFolderPath = Path.Combine(Environment.CurrentDirectory, "current");
                        return new LiteDbToJsonService(dbFilePath, outputFolderPath);
                    });
                    services.AddSingleton<PythonScriptExecutionService>();
                    services.AddSingleton<MergeDetectionService>(provider =>
                    {
                        string userName = Environment.UserName;
                        string dbFilePath = Path.Combine(@"C:\Users", userName, "AppData", "Local", "Laserwar", "alphatag", "alphatag.db");
                        string outputFolderPath = Path.Combine(Environment.CurrentDirectory, "current");
                        var liteDbToJsonService = provider.GetRequiredService<LiteDbToJsonService>();
                        var pythonScriptExecutionService = provider.GetRequiredService<PythonScriptExecutionService>();
                        return new MergeDetectionService(dbFilePath, outputFolderPath, liteDbToJsonService, pythonScriptExecutionService);
                    });
                })
                .UseWindowsService()
                .Build();

            await host.RunAsync();
        }
    }

    public class Worker : BackgroundService
    {
        private readonly MergeDetectionService _mergeDetectionService;
        private readonly ILogger<Worker> _logger;
        private Timer _timer;

        public Worker(MergeDetectionService mergeDetectionService, ILogger<Worker> logger)
        {
            _mergeDetectionService = mergeDetectionService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(10));

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            _timer?.Dispose();

            _logger.LogInformation("Worker service is stopping.");
        }

        private async void DoWork(object state)
        {
            try
            {
                await _mergeDetectionService.RunApplicationAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running application logic.");
            }
        }
    }
}