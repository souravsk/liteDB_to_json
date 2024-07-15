using Alphatag_Game.Services;
using System.Reflection;

namespace Alphatag_Game
{
    public class Program
    {
        static async Task Main()
        {
            // string userName = Environment.UserName;
            // string dbFilePath = Path.Combine(@"C:\Users", userName, "AppData", "Local", "Laserwar", "alphatag", "alphatag.db");
            string dbFilePath = @"/Users/sourav/Nexgensis/liteDB_to_json/sample_database/alphatag1.db";
            // string s3BucketName = "alphatag_db_json";
            // string executablePath = Path.GetFullPath(Assembly.GetExecutingAssembly().Location);
            // string outputFolderPath = Path.Combine(Environment.CurrentDirectory, "current");
            // //string zipFolderPath = Path.Combine(Environment.CurrentDirectory, "output_zip");
            // //string serverUrl = "http://154.49.243.244:3232/save_file";

            // var liteDbToJsonService = new LiteDbToJsonService(dbFilePath, s3BucketName, outputFolderPath);
            // await liteDbToJsonService.ConvertToJsonAndInsertToPostgresAsync();

            //var zipAndUploadService = new ZipAndUploadService(outputFolderPath, zipFolderPath, serverUrl);
            //zipAndUploadService.ZipOutputFolder();
            //await zipAndUploadService.UploadZipToServerAsync(Path.Combine(zipFolderPath, $"alphatag_data_{DateTime.Now.ToString("yyyyMMddHHmmss")}.zip"));

            // Run the Python script
            var pythonScriptExecutionService = new PythonScriptExecutionService();
            await pythonScriptExecutionService.RunPythonScriptAsync();
        }
    }
}