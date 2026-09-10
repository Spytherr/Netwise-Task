namespace Netwise_Task;

public interface ICatFactStore
{
    Task AppendAsync(CatFactDto catFact, CancellationToken cancellationToken);
}
