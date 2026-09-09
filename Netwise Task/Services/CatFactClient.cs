using System.Net.Http.Json;

namespace Netwise_Task;

public sealed class CatFactClient(HttpClient httpClient) : ICatFactClient
{
    public async Task<CatFactDto> GetAsync(CancellationToken cancellationToken)
    {
        var catFact = await httpClient.GetFromJsonAsync<CatFactDto>("fact", cancellationToken);

        return catFact ?? throw new InvalidOperationException("Cat Facts API returned an empty response.");
    }
}
