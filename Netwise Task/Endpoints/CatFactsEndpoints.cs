namespace Netwise_Task;

public static class CatFactsEndpoints
{
    public static void MapCatFactsEndpoints(this WebApplication app)
    {
        app.MapGet("/cat-facts", async (ICatFactService service, CancellationToken cancellationToken) =>
        {
            var catFact = await service.GetAndSaveAsync(cancellationToken);
            return Results.Ok(catFact);
        });
    }
}
