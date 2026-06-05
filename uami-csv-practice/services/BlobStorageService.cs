using Azure.Storage.Blobs;

namespace CsvUploadApi.Services;

public class BlobStorageService
{
    private readonly BlobContainerClient _containerClient;

    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureStorage:ConnectionString"];
        var containerName = configuration["AzureStorage:ContainerName"];

        var blobServiceClient = new BlobServiceClient(connectionString);

        _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {
        await _containerClient.CreateIfNotExistsAsync();

        var blobName = $"{Guid.NewGuid()}-{fileName}";

        var blobClient = _containerClient.GetBlobClient(blobName);

        await blobClient.UploadAsync(fileStream, overwrite: false);

        return blobClient.Uri.ToString();
    }
}