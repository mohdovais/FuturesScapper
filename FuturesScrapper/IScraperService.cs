namespace FuturesScrapper;

public interface IScraperService
{
    Task ExecuteAsync(Uri url, string csvFile, CancellationToken stoppingToken);
}
