namespace Netwise_Task;

public enum CatFactStorageProvider
{
    Unknown,
    File,
    AzureBlob
}

public sealed class CatFactStorageOptions
{
    public const string SectionName = "Storage";

    public CatFactStorageProvider Provider { get; init; }
    public FileCatFactStorageOptions File { get; init; } = new();
    public AzureBlobCatFactStorageOptions AzureBlob { get; init; } = new();
}

public sealed class FileCatFactStorageOptions
{
    public string Path { get; init; } = string.Empty;
}

public sealed class AzureBlobCatFactStorageOptions
{
    public string ServiceUri { get; init; } = string.Empty;
    public string ContainerName { get; init; } = string.Empty;
    public string BlobName { get; init; } = string.Empty;
}
