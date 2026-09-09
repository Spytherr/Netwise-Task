using Microsoft.Extensions.Options;
using Netwise_Task;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

builder.Services.AddOptions<CatFactOptions>()
    .Bind(builder.Configuration.GetRequiredSection(CatFactOptions.SectionName))
    .Validate(options =>
    {
        if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var uri))
            return false;

        return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
    }, "CatFacts:BaseUrl must be an absolute HTTP or HTTPS URL.")
    .Validate(options => !string.IsNullOrWhiteSpace(options.FilePath),
        "CatFacts:FilePath is required.")
    .ValidateOnStart();

builder.Services.AddSingleton<ICatFactFileWriter, CatFactFileWriter>();
builder.Services.AddScoped<ICatFactService, CatFactService>();
builder.Services.AddHttpClient<ICatFactClient, CatFactClient>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<CatFactOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
});

var app = builder.Build();

app.UseExceptionHandler();

app.MapCatFactsEndpoints();

app.Run();
