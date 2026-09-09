namespace Netwise_Task;

public interface ICatFactClient
{
    Task<CatFactDto> GetAsync(CancellationToken cancellationToken);
}
