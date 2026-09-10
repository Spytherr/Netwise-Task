using Netwise_Task;

var builder = WebApplication.CreateBuilder(args);

var catFactsBaseUrl = builder.Configuration["CatFacts:BaseUrl"]
    ?? throw new InvalidOperationException("CatFacts:BaseUrl is required.");
var catFactsBaseUri = new Uri(catFactsBaseUrl, UriKind.Absolute);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddCatFactStorage(builder.Configuration);
builder.Services.AddScoped<ICatFactService, CatFactService>();
builder.Services.AddHttpClient<ICatFactClient, CatFactClient>(client =>
{
    client.BaseAddress = catFactsBaseUri;
});

var app = builder.Build();

app.UseExceptionHandler();

app.MapCatFactsEndpoints();

app.Run();
