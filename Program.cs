using Alphatag_Game.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                })
                .UseWindowsService()
                .Build();

            await host.RunAsync();
        }
    }

    public class Worker : BackgroundService
    {
        private readonly MergeDetectionService _mergeDetectionService;

        public Worker()
        {   
            string userName = Environment.UserName;
            string dbFilePath = Path.Combine(@"C:\Users",userName, "AppData", "Local", "Laserwar", "alphatag", "alphatag.db");
           // string dbFilePath = @"/Users/sourav/Nexgensis/liteDB_to_json/sample_database/alphatag1.db";
            string outputFolderPath = Path.Combine(Environment.CurrentDirectory, "current");

            _mergeDetectionService = new MergeDetectionService(Path.GetDirectoryName(dbFilePath), outputFolderPath);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _mergeDetectionService.StartWatching();
            await Task.Delay(-1, stoppingToken);
        }
    }
}