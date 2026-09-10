using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;

namespace Netwise_Task;

public static class CatFactStorageExtensions
{
    public static IServiceCollection AddCatFactStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<CatFactStorageOptions>()
            .Bind(configuration.GetRequiredSection(CatFactStorageOptions.SectionName))
            .Validate(
                options => options.Provider != CatFactStorageProvider.File ||
                           !string.IsNullOrWhiteSpace(options.File.Path),
                "Storage:File:Path is required for the File provider.")
            .Validate(
                options => options.Provider != CatFactStorageProvider.AzureBlob ||
                           IsAbsoluteHttpUri(options.AzureBlob.ServiceUri),
                "Storage:AzureBlob:ServiceUri must be an absolute HTTP or HTTPS URL.")
            .Validate(
                options => options.Provider != CatFactStorageProvider.AzureBlob ||
                           !string.IsNullOrWhiteSpace(options.AzureBlob.ContainerName),
                "Storage:AzureBlob:ContainerName is required for the AzureBlob provider.")
            .Validate(
                options => options.Provider != CatFactStorageProvider.AzureBlob ||
                           !string.IsNullOrWhiteSpace(options.AzureBlob.BlobName),
                "Storage:AzureBlob:BlobName is required for the AzureBlob provider.")
            .ValidateOnStart();

        var provider = configuration.GetValue<CatFactStorageProvider>(
            $"{CatFactStorageOptions.SectionName}:Provider");

        switch (provider)
        {
            case CatFactStorageProvider.File:
                services.AddSingleton<ICatFactStore, FileCatFactStore>();
                break;

            case CatFactStorageProvider.AzureBlob:
                services.AddSingleton<TokenCredential>(_ => new DefaultAzureCredential());
                services.AddSingleton(serviceProvider =>
                {
                    var options = serviceProvider.GetRequiredService<IOptions<CatFactStorageOptions>>().Value;
                    var credential = serviceProvider.GetRequiredService<TokenCredential>();

                    return new BlobServiceClient(new Uri(options.AzureBlob.ServiceUri), credential);
                });
                services.AddSingleton<ICatFactStore, AzureBlobCatFactStore>();
                break;

            default:
                throw new InvalidOperationException($"Unsupported storage provider: {provider}.");
        }

        return services;
    }

    private static bool IsAbsoluteHttpUri(string value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
               (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
