using LiteDB;
using Newtonsoft.Json;

namespace Alphatag_Game.Services
{
    public class LiteDbToJsonService
    {
        private readonly string _dbFilePath;
        private readonly AwsS3Service _awsS3Service;

        public LiteDbToJsonService(string dbFilePath, string s3BucketName)
        {
            _dbFilePath = dbFilePath;
            _awsS3Service = new AwsS3Service(s3BucketName);
        }

        public void ConvertToJson()
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
                        _awsS3Service.UploadToS3(collectionName, json);
                    }
                }
            }
        }
    }
}