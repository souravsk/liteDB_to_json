using System;
using System.Diagnostics;
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

        public MergeDetectionService(string dbFilePath, string outputFolderPath, LiteDbToJsonService liteDbToJsonService, PythonScriptExecutionService pythonScriptExecutionService)
        {
            _dbFilePath = dbFilePath;
            _outputFolderPath = outputFolderPath;
            _liteDbToJsonService = liteDbToJsonService;
            _pythonScriptExecutionService = pythonScriptExecutionService;
        }

        public async Task RunApplicationAsync()
        {
            try
            {
                await _liteDbToJsonService.ConvertToJsonAndInsertToPostgresAsync();
                await _pythonScriptExecutionService.RunPythonScriptAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error running application logic: {ex.Message}");
            }
        }
    }
}