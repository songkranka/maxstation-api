namespace Vatno.Worker.BlobStorage
{
    public class AzureStorageSettings
    {
        public string ConnectionName { get; set; }
        public string BlobUrl { get; set; }
        public string SASToken { get; set; }
    }
}