using System;
using System.Diagnostics;
using System.IO;

namespace Alphatag_Game.Services
{
    public class MergeDetectionService
    {
        private readonly string _directoryToWatch;
        private readonly string _outputFolderPath;

        public MergeDetectionService(string directoryToWatch, string outputFolderPath)
        {
            _directoryToWatch = directoryToWatch;
            _outputFolderPath = outputFolderPath;
        }

        public void StartWatching()
        {
            FileSystemWatcher watcher = new FileSystemWatcher(_directoryToWatch);

            // Watch for changes in LastWrite times and the renaming of files or directories.
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;

            // Watch for changes in the database files.
            watcher.Filter = "*.db";

            // Add event handlers.
            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;

            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        private async void OnChanged(object source, FileSystemEventArgs e)
        {
            // Check if the merge event is detected (this is just an example, adjust as needed)
            if (e.Name == "alphatag.db")
            {
                Console.WriteLine("Merge detected. Running the application...");

                // Run the application logic
                await RunApplicationAsync();
            }
        }

        private async Task RunApplicationAsync()
        {
            try
            {
                string dbFilePath = Path.Combine(_directoryToWatch, "alphatag.db");
                string s3BucketName = "alphatag_db_json";
                
                var liteDbToJsonService = new LiteDbToJsonService(dbFilePath, s3BucketName, _outputFolderPath);
                await liteDbToJsonService.ConvertToJsonAndInsertToPostgresAsync();

                var pythonScriptExecutionService = new PythonScriptExecutionService();
                await pythonScriptExecutionService.RunPythonScriptAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error running application logic: {ex.Message}");
            }
        }
    }
}