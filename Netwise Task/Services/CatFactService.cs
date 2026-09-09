namespace Netwise_Task;

public sealed class CatFactService(
    ICatFactClient catFactClient,
    ICatFactFileWriter fileWriter) : ICatFactService
{
    public async Task<CatFactDto> GetAndSaveAsync(CancellationToken cancellationToken)
    {
        var catFact = await catFactClient.GetAsync(cancellationToken);
        await fileWriter.AppendAsync(catFact, cancellationToken);

        return catFact;
    }
}
