using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Alphatag_Game.Services
{
    public class GameMonitoringService
    {
        private readonly string _gameExecutableName;
        private readonly string _dbFilePath;
        private readonly string _outputFolderPath;
        private readonly ILogger<GameMonitoringService> _logger;
        private readonly LiteDbToJsonService _liteDbToJsonService;
        private readonly PythonScriptExecutionService _pythonScriptExecutionService;

        public GameMonitoringService(string gameExecutableName, string dbFilePath, string outputFolderPath, 
                                     ILogger<GameMonitoringService> logger, LiteDbToJsonService liteDbToJsonService,
                                     PythonScriptExecutionService pythonScriptExecutionService)
        {
            _gameExecutableName = gameExecutableName;
            _dbFilePath = dbFilePath;
            _outputFolderPath = outputFolderPath;
            _logger = logger;
            _liteDbToJsonService = liteDbToJsonService;
            _pythonScriptExecutionService = pythonScriptExecutionService;
        }

        public async Task MonitorGameAndConvertAsync()
        {
            _logger.LogInformation($"Monitoring game '{_gameExecutableName}' and converting LiteDB to JSON.");

            try
            {
                while (true)
                {
                    Process[] processes = Process.GetProcessesByName(_gameExecutableName);

                    if (processes.Length == 0)
                    {
                        _logger.LogInformation("Game process not found. Waiting for it to start...");
                        while (processes.Length == 0)
                        {
                            await Task.Delay(1000);
                            processes = Process.GetProcessesByName(_gameExecutableName);
                        }
                        _logger.LogInformation("Game process started. Waiting for it to exit...");
                    }

                    foreach (var process in processes)
                    {
                        process.EnableRaisingEvents = true;
                        process.Exited += async (sender, e) =>
                        {
                            _logger.LogInformation("Game process exited. Starting LiteDB to JSON conversion...");
                            try
                            {
                                await _liteDbToJsonService.ConvertToJsonAndSaveAsync();
                                _logger.LogInformation("LiteDB to JSON conversion and saving completed successfully.");

                                // After successful conversion, execute Python script
                                await _pythonScriptExecutionService.RunPythonScriptAsync();
                                _logger.LogInformation("Python script execution completed successfully.");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Error during processing: {ex.Message}");
                            }
                        };

                        process.WaitForExit();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error monitoring game process: {ex.Message}");
                throw; // Consider handling or logging specific exceptions
            }
            finally
            {
                _logger.LogInformation("Game process monitoring ended.");
            }
        }
    }
}
