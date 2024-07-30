using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Application.Services.ImageStorage
{


    public class StorageService : IStorageService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName = "e-commerce-image-bucket"; 

        public StorageService(IConfiguration configuration)
        {
            var credentialPath = configuration["GoogleCloud:CredentialPath"];
            if (string.IsNullOrEmpty(credentialPath))
            {
                throw new InvalidOperationException("Google Cloud credentials not found. Ensure the GOOGLE_APPLICATION_CREDENTIALS environment variable is set.");
            }

            GoogleCredential googleCredential;
            using (var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read))
            {
                googleCredential = GoogleCredential.FromStream(stream);
            }

            _storageClient = StorageClient.Create(googleCredential);
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var result = await _storageClient.UploadObjectAsync(_bucketName, fileName, null, memoryStream);
            return result.MediaLink;
        }

        public async Task<bool> DeleteFileAsync(string fileName)
        {

            try
            {
                await _storageClient.DeleteObjectAsync(_bucketName, fileName);
                return true;
            }
            catch (Google.GoogleApiException ex) when (ex.Error.Code == 404)
            {
                // Log the exception (you can use a logging framework)
                Console.WriteLine($"File not found: {fileName}");
                return false;
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework)
                Console.WriteLine($"Error deleting file: {ex.Message}");
                throw new ApplicationException("An error occurred while deleting the file.", ex);
            }


        }
    }
}
