namespace FuturesScapper;

public interface IScraperService
{
    Task ExecuteAsync(string url, string csvFile, CancellationToken stoppingToken);
}
