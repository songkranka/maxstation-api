using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;

namespace Vatno.Worker.BlobStorage
{
    public interface IBlobStorageService
    {
        Task<Uri> UploadAsync(string containerName, string pathToSave, IFormFile file, bool? publish = null);

        Task<Uri> GetMediaAsync(string containerName, string pathToSave);

        void DeleteMediaAsync(string containerName, string pathToDelete);

        Task<BlobDownloadInfo> DownloadAsync(string containerName, string pathToSave);

        Task<Uri> GetBlobContainerAsync(string containerName);
        Task<Uri> DownloadFileUploadToAsync(string blobUrl, string containerName, string pathToSave);

        Task<BlobClient> GetBlockBlobReferenceAsync(string containerName, string name);
        Task<Uri> SASUploadAsync(string containerName, IFormFile file);
        IFormFile ConvertStringToIFormFile(string content, string fileName = null, string contentType = null);
    }
}