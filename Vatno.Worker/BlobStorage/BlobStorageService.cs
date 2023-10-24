using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;

namespace Vatno.Worker.BlobStorage
{
    /// <inheritdoc />
    public class BlobStorageService : IBlobStorageService
    {
        private readonly IConfiguration _configuration;
        private readonly AzureStorageSettings _blobStorageSetting;


        /// <inheritdoc />
        public BlobStorageService(IOptions<AzureStorageSettings> blobStorageSetting, IConfiguration configuration)
        {
            _configuration = configuration;
            _blobStorageSetting = blobStorageSetting.Value;

        }

        /// <summary>
        /// Upload
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="pathToSave"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<System.Uri> UploadAsync(string containerName, string pathToSave, IFormFile file,
            bool? publish = null)
        {
            var blobConnectionString = _configuration.GetValue<string>(_blobStorageSetting.ConnectionName);
            BlobServiceClient serviceClient = new BlobServiceClient(blobConnectionString);
            BlobContainerClient containerClient = await CreateBlobContainerIfNotExistAsync(serviceClient, containerName, publish);
            BlobClient blobClient = containerClient.GetBlobClient(pathToSave);
            BlobUploadOptions options = new BlobUploadOptions
            { HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType } };
            Response<BlobContentInfo> result;
            using (var fileStream = file.OpenReadStream())
            {
                result = await blobClient.UploadAsync(fileStream, options);
            }

            return blobClient.Uri;
        }

        private async Task<BlobContainerClient> CreateBlobContainerIfNotExistAsync(BlobServiceClient serviceClient,
            string containerName, bool? publish = null)
        {
            if (serviceClient.GetBlobContainers().Any(o => o.Name == containerName))
            {
                return serviceClient.GetBlobContainerClient(containerName);
            }

            if (publish == null || !publish.Value)
            {
                return await serviceClient.CreateBlobContainerAsync(containerName);
            }

            return await serviceClient.CreateBlobContainerAsync(containerName, PublicAccessType.Blob);
        }


        public async Task<System.Uri> GetMediaAsync(string containerName, string pathToSave)
        {
            var blobConnectionString = _configuration.GetValue<string>(_blobStorageSetting.ConnectionName);
            BlobServiceClient serviceClient = new BlobServiceClient(blobConnectionString);
            BlobContainerClient containerClient =
                await CreateBlobContainerIfNotExistAsync(serviceClient, containerName);
            BlobClient blobClient = containerClient.GetBlobClient(pathToSave);

            if (await blobClient.ExistsAsync())
                return blobClient.Uri;
            else
                return null;
        }

        public async void DeleteMediaAsync(string containerName, string pathToDelete)
        {
            var blobConnectionString = _configuration.GetValue<string>(_blobStorageSetting.ConnectionName);
            BlobServiceClient serviceClient = new BlobServiceClient(blobConnectionString);
            BlobContainerClient containerClient =
                await CreateBlobContainerIfNotExistAsync(serviceClient, containerName);
            BlobClient blobClient = containerClient.GetBlobClient(pathToDelete);

            await blobClient.DeleteIfExistsAsync();
        }

        public async Task<BlobDownloadInfo> DownloadAsync(string containerName, string pathToSave)
        {
            var blobConnectionString = _configuration.GetValue<string>(_blobStorageSetting.ConnectionName);
            BlobServiceClient serviceClient = new BlobServiceClient(blobConnectionString);
            BlobContainerClient containerClient =
                await CreateBlobContainerIfNotExistAsync(serviceClient, containerName);
            BlobClient blobClient = containerClient.GetBlobClient(pathToSave);

            if (await blobClient.ExistsAsync())
                return await blobClient.DownloadAsync();
            else
                return null;
        }

        public async Task<System.Uri> GetBlobContainerAsync(string containerName)
        {
            var blobConnectionString = _configuration.GetValue<string>(_blobStorageSetting.ConnectionName);
            BlobServiceClient serviceClient = new BlobServiceClient(blobConnectionString);
            BlobContainerClient containerClient =
                await CreateBlobContainerIfNotExistAsync(serviceClient, containerName);

            return containerClient.Uri;
        }

        public async Task<System.Uri> DownloadFileUploadToAsync(string blobUrl, string containerName, string pathToSave)
        {
            WebClient wc = new WebClient();
            using (MemoryStream stream = new MemoryStream(wc.DownloadData(blobUrl)))
            {
                stream.Position = 0;
                var image = new FormFile(stream, 0, stream.Length, pathToSave, pathToSave)
                { Headers = new HeaderDictionary(), ContentType = "image/jpg" };
                ;
                return await UploadAsync(containerName, pathToSave, image);
            }
        }

        public async Task<BlobClient> GetBlockBlobReferenceAsync(string containerName, string name)
        {
            var blobConnectionString = _configuration.GetValue<string>(_blobStorageSetting.ConnectionName);
            BlobServiceClient serviceClient = new BlobServiceClient(blobConnectionString);
            BlobContainerClient containerClient =
                await CreateBlobContainerIfNotExistAsync(serviceClient, containerName);

            return containerClient.GetBlobClient(name);
        }


        public async Task<System.Uri> SASUploadAsync(string pathToSave, IFormFile file)
        {
            var blobContainerUri = new Uri(_blobStorageSetting.BlobUrl);
            var sasCredential = new AzureSasCredential(_blobStorageSetting.SASToken);
            var containerClient = new BlobContainerClient(blobContainerUri, sasCredential);

            BlobClient blobClient = containerClient.GetBlobClient(pathToSave);
            BlobUploadOptions options = new BlobUploadOptions
            { HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType } };
            Response<BlobContentInfo> result;
            using (var fileStream = file.OpenReadStream())
            {
                result = await blobClient.UploadAsync(fileStream, options);
            }

            return blobClient.Uri;
        }

        public IFormFile ConvertStringToIFormFile(string content, string fileName = null, string contentType = null)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(content);
            MemoryStream stream = new MemoryStream(byteArray);

            return new FormFile(stream, 0, stream.Length, null, fileName)
            { Headers = new HeaderDictionary(), ContentType = contentType ?? "text/plain" };
        }
    }
}