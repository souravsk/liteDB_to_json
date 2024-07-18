using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using Alphatag_Game.Services;

namespace Alphatag_Game
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    string userName = Environment.UserName;
                    string dbFilePath = Path.Combine(@"C:\Users", userName, "AppData", "Local", "Laserwar", "alphatag", "alphatag.db");
                    string outputFolderPath = Path.Combine(Environment.CurrentDirectory, "current");

                    services.AddSingleton<GameMonitoringService>();
                    services.AddSingleton<LiteDbToJsonService>(provider => 
                        new LiteDbToJsonService(dbFilePath, outputFolderPath));
                    
                    services.AddSingleton(provider =>
                    {
                        var logger = provider.GetRequiredService<ILogger<GameMonitoringService>>();
                        var liteDbToJsonService = provider.GetRequiredService<LiteDbToJsonService>();
                        var pythonScriptExecutionService = provider.GetRequiredService<PythonScriptExecutionService>();
                        return new GameMonitoringService("AlphaTag", dbFilePath, outputFolderPath, logger, liteDbToJsonService, pythonScriptExecutionService);
                    });

                    services.AddSingleton<PythonScriptExecutionService>();
                })
                .UseWindowsService() // Use this for Windows service hosting
                .Build();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Application is starting.");

            try
            {
                // Start the host
                await host.StartAsync();

                // Start monitoring the game and converting LiteDB to JSON
                var gameMonitoringService = host.Services.GetRequiredService<GameMonitoringService>();
                await gameMonitoringService.MonitorGameAndConvertAsync();

                // Keep the application running until terminated
                await host.WaitForShutdownAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while running the application.");
            }
            finally
            {
                // Ensure to gracefully stop the host
                if (host is IAsyncDisposable asyncDisposableHost)
                {
                    await asyncDisposableHost.DisposeAsync();
                }
                else if (host is IDisposable disposableHost)
                {
                    disposableHost.Dispose();
                }
            }
        }
    }
}
