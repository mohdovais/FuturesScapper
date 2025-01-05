using FuturesScapper;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IScraperService, ScraperService>();
builder.Services.AddHttpClient<IScraperService, ScraperService>();

var host = builder.Build();
host.Run();
