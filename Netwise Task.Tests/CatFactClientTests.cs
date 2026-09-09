using System.Net;
using System.Text;
using Netwise_Task;

namespace Netwise_Task.Tests;

public class CatFactClientTests
{
    [Fact]
    public async Task GetAsync_ShouldCallConfiguredEndpointAndDeserializeResponse()
    {
        const string responseJson = """
            {"fact":"Cats sleep for most of the day.","length":35}
            """;
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };
        using var handler = new StubHttpMessageHandler(response);
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://catfact.ninja/")
        };
        var client = new CatFactClient(httpClient);

        var result = await client.GetAsync(CancellationToken.None);

        Assert.Equal(new CatFactDto("Cats sleep for most of the day.", 35), result);
        Assert.Equal(new Uri("https://catfact.ninja/fact"), handler.RequestUri);
    }

    private sealed class StubHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
    {
        public Uri? RequestUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestUri = request.RequestUri;
            return Task.FromResult(response);
        }
    }
}
