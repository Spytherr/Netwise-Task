using Netwise_Task;

namespace Netwise_Task.Tests;

public class CatFactServiceTests
{
    [Fact]
    public async Task GetAndSaveAsync_ShouldSaveAndReturnFetchedFact()
    {
        var expectedFact = new CatFactDto("Cats have five toes on their front paws.", 40);
        var client = new StubCatFactClient(expectedFact);
        var writer = new RecordingCatFactFileWriter();
        var service = new CatFactService(client, writer);

        var result = await service.GetAndSaveAsync(CancellationToken.None);

        Assert.Same(expectedFact, result);
        Assert.Same(expectedFact, writer.SavedFact);
    }

    private sealed class StubCatFactClient(CatFactDto catFact) : ICatFactClient
    {
        public Task<CatFactDto> GetAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(catFact);
        }
    }

    private sealed class RecordingCatFactFileWriter : ICatFactFileWriter
    {
        public CatFactDto? SavedFact { get; private set; }

        public Task AppendAsync(CatFactDto catFact, CancellationToken cancellationToken)
        {
            SavedFact = catFact;
            return Task.CompletedTask;
        }
    }
}
