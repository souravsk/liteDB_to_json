using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace Alphatag_Game.Services
{
    public class ZipAndUploadService
    {
        private readonly string _outputFolderPath;
        private readonly string _zipFolderPath;
        private readonly string _serverUrl;

        public ZipAndUploadService(string outputFolderPath, string zipFolderPath, string serverUrl)
        {
            _outputFolderPath = outputFolderPath;
            _zipFolderPath = zipFolderPath;
            _serverUrl = serverUrl;
        }

        public void ZipAndUploadToServer()
        {
            string zipFileName = $"alphatag_data_{DateTime.Now.ToString("yyyyMMddHHmmss")}.zip";
            string zipFilePath = Path.Combine(_zipFolderPath, zipFileName);

            ZipDirectory(_outputFolderPath, zipFilePath);

            // Upload the zip file to the server
            UploadZipToServerAsync(zipFilePath).GetAwaiter().GetResult();

            // Delete the zip file after upload
            File.Delete(zipFilePath);
        }

        private void ZipDirectory(string sourceDirectory, string zipFilePath)
        {
            // Create the output_zip folder if it doesn't exist
            Directory.CreateDirectory(_zipFolderPath);

            using (var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                foreach (var file in Directory.GetFiles(sourceDirectory))
                {
                    zipArchive.CreateEntryFromFile(file, Path.GetFileName(file));
                }
            }
        }

        public async Task UploadZipToServerAsync(string zipFilePath)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(5);

                    // Check the connection to the server URL
                    var connectionCheckResponse = await client.GetAsync(_serverUrl);
                    if (!connectionCheckResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Error checking connection to server URL: {connectionCheckResponse.StatusCode} - {connectionCheckResponse.ReasonPhrase}");
                        return;
                    }

                    using (var content = new MultipartFormDataContent())
                    {
                        content.Add(new ByteArrayContent(File.ReadAllBytes(zipFilePath)), "file", Path.GetFileName(zipFilePath));
                        Console.WriteLine($"Request content: {await content.ReadAsStringAsync()}");
                        var response = await client.PostAsync(_serverUrl, content);

                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Zip file uploaded successfully.");
                            Console.WriteLine($"Server response: {await response.Content.ReadAsStringAsync()}");
                        }
                        else
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"Error uploading zip file: {response.StatusCode} - {response.ReasonPhrase}\nResponse content: {responseContent}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading zip file: {ex.Message}");
            }
        }
    }
}