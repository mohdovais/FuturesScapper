using System.Text.Json.Serialization;
using FuturesScrapper;
using FuturesScrapper.Models;
using Polly;
using Polly.Extensions.Http;

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;

services.Configure<Settings>(builder.Configuration.GetSection(Settings.Name));
services.AddHostedService<Worker>();
services.AddSingleton<IScraperService, ScraperService>();
services.AddHttpClient<IScraperService, ScraperService>().SetHandlerLifetime(TimeSpan.FromMinutes(5)).AddPolicyHandler(GetRetryPolicy());

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var host = builder.Build();
host.Run();

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Settings))]
[JsonSerializable(typeof(PageTitle))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}
