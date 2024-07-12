using LiteDB;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Alphatag_Game.Services
{
    public class LiteDbToJsonService
    {
        private readonly string _dbFilePath;
        private readonly string _s3BucketName;
        private readonly string _connectionString;
        private readonly string _outputFolderPath;
        private readonly AwsS3Service _awsS3Service;

        public LiteDbToJsonService(string dbFilePath, string s3BucketName, string connectionString, string outputFolderPath)
        {
            _dbFilePath = dbFilePath;
            _s3BucketName = s3BucketName;
            _connectionString = connectionString;
            _outputFolderPath = outputFolderPath;
            _awsS3Service = new AwsS3Service(s3BucketName);
        }

        public async Task ConvertToJsonAndInsertToPostgresAsync()
        {
            using (var db = new LiteDatabase(_dbFilePath))
            {
                var jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.Converters.Add(new BsonValueConverter());

                var collectionNames = db.GetCollectionNames();

                foreach (var collectionName in collectionNames)
                {
                    var collection = db.GetCollection(collectionName);
                    var items = collection.FindAll();

                    if (items.Any())
                    {
                        string json = JsonConvert.SerializeObject(items, Formatting.Indented, jsonSerializerSettings);
                        // await _awsS3Service.UploadToS3Async(collectionName, json);
                        // await InsertToPostgresAsync(collectionName, json);
                        await SaveToLocalFileAsync(collectionName, json);
                    }
                }
            }
        }

        private async Task InsertToPostgresAsync(string tableName, string json)
        {
            // Code to insert JSON data into PostgreSQL using the _connectionString
            // ...
        }

        private async Task SaveToLocalFileAsync(string fileName, string json)
        {
            string filePath = Path.Combine(_outputFolderPath, $"{fileName}.json");
            Directory.CreateDirectory(_outputFolderPath);
            await File.WriteAllTextAsync(filePath, json);
        }
    }
}