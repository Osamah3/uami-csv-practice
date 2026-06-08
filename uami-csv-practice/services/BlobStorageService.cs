using Azure.Identity;
using Azure.Storage.Blobs;

namespace CsvUploadApi.Services;

public class BlobStorageService
{
    private readonly BlobContainerClient _containerClient;

    public BlobStorageService(IConfiguration configuration)
    {
        var blobServiceUri = configuration["AzureStorage:BlobServiceUri"];
        var containerName = configuration["AzureStorage:ContainerName"];

        if (string.IsNullOrEmpty(blobServiceUri))
            throw new InvalidOperationException("AzureStorage:BlobServiceUri is missing.");

        if (string.IsNullOrEmpty(containerName))
            throw new InvalidOperationException("AzureStorage:ContainerName is missing.");

        var credential = new DefaultAzureCredential(
            new DefaultAzureCredentialOptions
            {
                ManagedIdentityClientId =
                    configuration["AzureStorage:ManagedIdentityClientId"]
            });

        var blobServiceClient = new BlobServiceClient(
            new Uri(blobServiceUri),
            credential);

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