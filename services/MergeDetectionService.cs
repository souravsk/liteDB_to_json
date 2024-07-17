using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Alphatag_Game.Services
{
    public class MergeDetectionService
    {
        private readonly string _dbFilePath;
        private readonly string _outputFolderPath;
        private readonly LiteDbToJsonService _liteDbToJsonService;
        private readonly PythonScriptExecutionService _pythonScriptExecutionService;
        private readonly TimeSpan _retryInterval = TimeSpan.FromMinutes(10);

        public MergeDetectionService(string dbFilePath, string outputFolderPath, LiteDbToJsonService liteDbToJsonService, PythonScriptExecutionService pythonScriptExecutionService)
        {
            _dbFilePath = dbFilePath;
            _outputFolderPath = outputFolderPath;
            _liteDbToJsonService = liteDbToJsonService;
            _pythonScriptExecutionService = pythonScriptExecutionService;
        }

        public async Task RunApplicationAsync()
        {
            while (true)
            {
                try
                {
                    if (IsFileLocked(_dbFilePath))
                    {
                        Console.WriteLine($"Database file '{_dbFilePath}' is in use. Retrying in {_retryInterval.TotalMinutes} minutes.");
                        await Task.Delay(_retryInterval);
                        continue;
                    }

                    await _liteDbToJsonService.ConvertToJsonAndInsertToPostgresAsync();
                    await _pythonScriptExecutionService.RunPythonScriptAsync();
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error running application logic: {ex.Message}");
                    await Task.Delay(_retryInterval);
                }
            }
        }

        private bool IsFileLocked(string filePath)
        {
            try
            {
                using (File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return false;
                }
            }
            catch (IOException)
            {
                return true;
            }
        }
    }
}