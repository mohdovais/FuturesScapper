namespace FuturesScrapper;

public interface IScraperService
{
    Task ExecuteAsync(Uri url, string csvFile, bool overwrite, CancellationToken stoppingToken);
}
