namespace Netwise_Task;

public interface ICatFactService
{
    Task<CatFactDto> GetAndSaveAsync(CancellationToken cancellationToken);
}
