using System.Text.Json.Serialization;
using FuturesScrapper;
using FuturesScrapper.Models;

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;

services.Configure<Settings>(builder.Configuration.GetSection(Settings.Name));
services.AddHostedService<Worker>();
services.AddSingleton<IScraperService, ScraperService>();
services.AddHttpClient<IScraperService, ScraperService>();

var host = builder.Build();
host.Run();

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Settings))]
[JsonSerializable(typeof(PageTitle))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}
