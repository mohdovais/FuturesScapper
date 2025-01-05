using FuturesScrapper;
using FuturesScrapper.Models;

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;;

services.Configure<Settings>(builder.Configuration.GetSection(Settings.Name));
services.AddHostedService<Worker>();
services.AddSingleton<IScraperService, ScraperService>();
services.AddHttpClient<IScraperService, ScraperService>();

var host = builder.Build();
host.Run();
