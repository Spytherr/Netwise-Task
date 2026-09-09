namespace Netwise_Task;

public interface ICatFactFileWriter
{
    Task AppendAsync(CatFactDto catFact, CancellationToken cancellationToken);
}
