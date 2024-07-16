using LiteDB;
using Newtonsoft.Json;

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

        // Converting database into json and then saving in to file
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
                        await SaveToLocalFileAsync(collectionName, json);
                    }
                }
            }
        }

        // saving the json files in the folder
        private async Task SaveToLocalFileAsync(string fileName, string json)
        {
            string currentFilePath = Path.Combine(_outputFolderPath, $"{fileName}.json");

            Directory.CreateDirectory(_outputFolderPath);

            await File.WriteAllTextAsync(currentFilePath, json);
        }
    }
}