namespace Netwise_Task;

public sealed class CatFactService(
    ICatFactClient catFactClient,
    ICatFactStore catFactStore) : ICatFactService
{
    public async Task<CatFactDto> GetAndSaveAsync(CancellationToken cancellationToken)
    {
        var catFact = await catFactClient.GetAsync(cancellationToken);
        await catFactStore.AppendAsync(catFact, cancellationToken);

        return catFact;
    }
}
