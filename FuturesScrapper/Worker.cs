using FuturesScrapper.Models;
using Microsoft.Extensions.Options;

namespace FuturesScrapper;

public sealed class Worker: BackgroundService
{
    private readonly Settings _settings;
    private readonly ILogger<Worker> _logger;
    private readonly IScraperService _service;

    public Worker( ILogger<Worker> logger, IOptions<Settings> options, IScraperService scraperService){
        _logger = logger;
        _settings = options.Value;
        _service = scraperService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var url = _settings.Url;
        var csvFile = _settings.CsvFile;
        var delay = _settings.DelayInMinutes;

        if(url == null){
            _logger.LogCritical("'Url' is missing in appsettings.json");
            Environment.Exit(1);
            return;
        }

        if(string.IsNullOrEmpty(csvFile)){
            _logger.LogCritical("'CsvFile' is missing in appsettings.json");
            Environment.Exit(1);
            return;
        }
        
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Starting now");
                await _service.ExecuteAsync(url, csvFile, stoppingToken);
                _logger.LogInformation("Completed");

                await Task.Delay(TimeSpan.FromMinutes(delay), stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            // When the stopping token is canceled, for example, a call made from services.msc,
            // we shouldn't exit with a non-zero exit code. In other words, this is expected...
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);

            // Terminates this process and returns an exit code to the operating system.
            // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
            // performs one of two scenarios:
            // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
            // 2. When set to "StopHost": will cleanly stop the host, and log errors.
            //
            // In order for the Windows Service Management system to leverage configured
            // recovery options, we need to terminate the process with a non-zero exit code.
            Environment.Exit(1);
        }
    }
}
