using System.Text;
using System.Text.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Extensions.Options;

namespace Netwise_Task;

public sealed class AzureBlobCatFactStore : ICatFactStore, IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly BlobContainerClient containerClient;
    private readonly AppendBlobClient appendBlobClient;
    private readonly ILogger<AzureBlobCatFactStore> logger;
    private readonly SemaphoreSlim initializationLock = new(1, 1);
    private volatile bool initialized;

    public AzureBlobCatFactStore(
        BlobServiceClient blobServiceClient,
        IOptions<CatFactStorageOptions> options,
        ILogger<AzureBlobCatFactStore> logger)
    {
        var azureBlobOptions = options.Value.AzureBlob;
        containerClient = blobServiceClient.GetBlobContainerClient(azureBlobOptions.ContainerName);
        appendBlobClient = containerClient.GetAppendBlobClient(azureBlobOptions.BlobName);
        this.logger = logger;
    }

    public async Task AppendAsync(CatFactDto catFact, CancellationToken cancellationToken)
    {
        await EnsureInitializedAsync(cancellationToken);

        var line = JsonSerializer.Serialize(catFact, JsonOptions) + "\n";
        using var content = new MemoryStream(Encoding.UTF8.GetBytes(line), writable: false);

        await appendBlobClient.AppendBlockAsync(content, cancellationToken: cancellationToken);
    }

    private async Task EnsureInitializedAsync(CancellationToken cancellationToken)
    {
        if (initialized)
            return;

        await initializationLock.WaitAsync(cancellationToken);
        try
        {
            if (initialized)
                return;

            await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            await appendBlobClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            initialized = true;

            logger.LogInformation(
                "Azure Append Blob storage initialized for container {ContainerName} and blob {BlobName}.",
                containerClient.Name,
                appendBlobClient.Name);
        }
        finally
        {
            initializationLock.Release();
        }
    }

    public void Dispose()
    {
        initializationLock.Dispose();
    }
}
