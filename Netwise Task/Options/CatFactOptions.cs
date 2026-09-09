namespace Netwise_Task;

public sealed class CatFactOptions
{
    public const string SectionName = "CatFacts";

    public string BaseUrl { get; init; } = string.Empty;
    public string FilePath { get; init; } = string.Empty;
}
