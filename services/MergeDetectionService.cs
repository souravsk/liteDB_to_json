using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Alphatag_Game.Services
{
    public class MergeDetectionService
    {
       
        private readonly string _dbFilePath;
        private readonly string _outputFolderPath;
        private readonly LiteDbToJsonService _liteDbToJsonService;
        private readonly PythonScriptExecutionService _pythonScriptExecutionService;
        private readonly GameMonitoringService _gameMonitoringService;
        private readonly ILogger<MergeDetectionService> _logger;
        private readonly TimeSpan _retryInterval = TimeSpan.FromMinutes(10);

        public MergeDetectionService(
            string dbFilePath,
            string outputFolderPath,
            LiteDbToJsonService liteDbToJsonService,
            PythonScriptExecutionService pythonScriptExecutionService,
            GameMonitoringService gameMonitoringService,
            ILogger<MergeDetectionService> logger)
        {
            _dbFilePath = dbFilePath;
            _outputFolderPath = outputFolderPath;
            _liteDbToJsonService = liteDbToJsonService;
            _pythonScriptExecutionService = pythonScriptExecutionService;
            _gameMonitoringService = gameMonitoringService;
            _logger = logger;
        }


        public async Task RunApplicationAsync()
        {
            try
            {
                await _liteDbToJsonService.ConvertToJsonAndSaveAsync();
                await _pythonScriptExecutionService.RunPythonScriptAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error running merge detection: {ex.Message}");
                throw;
            }
        }
    }
}
