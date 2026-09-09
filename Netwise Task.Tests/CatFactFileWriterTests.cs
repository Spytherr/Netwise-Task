using System.Text.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Netwise_Task;

namespace Netwise_Task.Tests;

public class CatFactFileWriterTests
{
    [Fact]
    public async Task AppendAsync_ShouldCreateFileAndWriteEachFactOnNewLine()
    {
        var contentRootPath = Path.Combine(Path.GetTempPath(), $"netwise-task-{Guid.NewGuid():N}");
        var relativeFilePath = Path.Combine("Data", "cat-facts.txt");
        var fullFilePath = Path.Combine(contentRootPath, relativeFilePath);
        var options = Options.Create(new CatFactOptions
        {
            BaseUrl = "https://catfact.ninja/",
            FilePath = relativeFilePath
        });
        var environment = new TestHostEnvironment(contentRootPath);
        var firstFact = new CatFactDto("First fact", 10);
        var secondFact = new CatFactDto("Second fact", 11);

        try
        {
            using (var writer = new CatFactFileWriter(options, environment))
            {
                await writer.AppendAsync(firstFact, CancellationToken.None);
                await writer.AppendAsync(secondFact, CancellationToken.None);
            }

            var lines = await File.ReadAllLinesAsync(fullFilePath);
            var savedFacts = lines
                .Select(line => JsonSerializer.Deserialize<CatFactDto>(line, JsonSerializerOptions.Web))
                .ToArray();

            Assert.Equal(2, lines.Length);
            Assert.Equal(new CatFactDto?[] { firstFact, secondFact }, savedFacts);
        }
        finally
        {
            if (Directory.Exists(contentRootPath))
                Directory.Delete(contentRootPath, true);
        }
    }

    private sealed class TestHostEnvironment(string contentRootPath) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Development;
        public string ApplicationName { get; set; } = "Netwise Task.Tests";
        public string ContentRootPath { get; set; } = contentRootPath;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
