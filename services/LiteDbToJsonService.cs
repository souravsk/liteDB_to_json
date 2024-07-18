using Newtonsoft.Json;
using LiteDB;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Alphatag_Game.Services
{
    public class LiteDbToJsonService
    {
        private readonly string _dbFilePath;
        private readonly string _outputFolderPath;

        public LiteDbToJsonService(string dbFilePath, string outputFolderPath)
        {
            _dbFilePath = dbFilePath;
            _outputFolderPath = outputFolderPath;
        }

        public async Task ConvertToJsonAndSaveAsync()
        {
            // Ensure output directory exists
            Directory.CreateDirectory(_outputFolderPath);

            using (var db = new LiteDatabase(_dbFilePath))
            {
                var collectionNames = db.GetCollectionNames();

                foreach (var collectionName in collectionNames)
                {
                    var collection = db.GetCollection(collectionName);
                    var items = collection.FindAll();

                    if (items.Any())
                    {
                        string jsonFilePath = Path.Combine(_outputFolderPath, $"{collectionName}.json");

                        // Use JsonSerializerSettings to configure serialization
                        var settings = new JsonSerializerSettings
                        {
                            Converters = { new BsonValueConverter() },
                            Formatting = Formatting.Indented
                        };

                        string json = JsonConvert.SerializeObject(items, settings);
                        await File.WriteAllTextAsync(jsonFilePath, json);
                    }
                }
            }
        }
    }
}
