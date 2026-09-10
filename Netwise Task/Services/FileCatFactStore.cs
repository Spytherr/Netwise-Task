using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Netwise_Task;

public sealed class FileCatFactStore : ICatFactStore, IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly SemaphoreSlim writeLock = new(1, 1);
    private readonly string filePath;

    public FileCatFactStore(IOptions<CatFactStorageOptions> options, IHostEnvironment environment)
    {
        filePath = Path.GetFullPath(options.Value.File.Path, environment.ContentRootPath);

        var directoryPath = Path.GetDirectoryName(filePath);
        if (directoryPath is not null)
            Directory.CreateDirectory(directoryPath);

        using var file = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
    }

    public async Task AppendAsync(CatFactDto catFact, CancellationToken cancellationToken)
    {
        var line = JsonSerializer.Serialize(catFact, JsonOptions);

        await writeLock.WaitAsync(cancellationToken);
        try
        {
            await File.AppendAllTextAsync(filePath, line + Environment.NewLine, cancellationToken);
        }
        finally
        {
            writeLock.Release();
        }
    }

    public void Dispose()
    {
        writeLock.Dispose();
    }
}
