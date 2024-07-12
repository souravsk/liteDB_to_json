using Alphatag_Game.Services;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Alphatag_Game
{
    public class Program
    {
        static async Task Main()
        {
            string userName = Environment.UserName;
            string dbFilePath = Path.Combine(@"C:\Users", userName, "AppData", "Local", "Laserwar", "alphatag", "alphatag.db");
            string s3BucketName = "alphatag_db_json";
            string executablePath = Path.GetFullPath(Assembly.GetExecutingAssembly().Location);
            string connectionString = "Host=localhost;Database=my_database;Username=my_user;Password=my_password";
            string outputFolderPath = Path.Combine(Environment.CurrentDirectory, "output");
            string zipFolderPath = Path.Combine(Environment.CurrentDirectory, "output_zip");
            string serverUrl = "http://154.49.243.244:3232/save_file";

            var liteDbToJsonService = new LiteDbToJsonService(dbFilePath, s3BucketName, connectionString, outputFolderPath);
            await liteDbToJsonService.ConvertToJsonAndInsertToPostgresAsync();

            var zipAndUploadService = new ZipAndUploadService(outputFolderPath, zipFolderPath, serverUrl);
            string zipFilePath = Path.Combine(zipFolderPath, $"alphatag_data_{DateTime.Now.ToString("yyyyMMddHHmmss")}.zip");
            await ZipAndUploadService.ZipAndUploadToServers();
        }
    }
}