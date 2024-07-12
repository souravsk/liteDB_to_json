using Amazon.S3;
using Amazon.S3.Model;

namespace Alphatag_Game.Services
{
    public class AwsS3Service
    {
        private readonly string _bucketName;

        public AwsS3Service(string bucketName)
        {
            _bucketName = bucketName;
        }

        public void UploadToS3(string collectionName, string json)
        {
            var s3Client = new AmazonS3Client();
            var key = $"{collectionName}.json";

            var putObjectRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                ContentBody = json
            };

            s3Client.PutObjectAsync(putObjectRequest).Wait();
            Console.WriteLine($"Data from collection '{collectionName}' uploaded to S3 successfully.");
        }
    }
}