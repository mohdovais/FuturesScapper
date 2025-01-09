namespace FuturesScrapper;

public sealed class ScraperService(ILogger<IScraperService> logger, HttpClient httpClient) : IScraperService
{
    public async Task ExecuteAsync(Uri url, string csvFile, CancellationToken stoppingToken)
    {
        var content = await GetUrlContentAsync(url, stoppingToken);

        if (string.IsNullOrEmpty(content))
        {
            logger.LogError("Empty content received from remote web server");
            return;
        }

        var result = Parser.Parse(content);

        if (result == null)
        {
            logger.LogError("Could not parse HTML");
            return;
        }

        WriteCsv(csvFile, GetCsvLine(result));
    }

    private async Task<string?> GetUrlContentAsync(Uri url, CancellationToken stoppingToken)
    {
        using var response = await httpClient.GetAsync(url, stoppingToken);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(stoppingToken);

        return content;
    }

    private void WriteCsv(string path, string content)
    {

        if (string.IsNullOrEmpty(path))
        {
            logger.LogError("Cannot write CSV as path is empty");
            return;
        }

        bool empty = false;

        if (!File.Exists(path))
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            empty = true;
        }

        using var write = File.AppendText(path);

        if (empty)
        {
            write.WriteLine("\"Trade Time\",\"TimeStamp\",\"Last Price\",\"Put Premium Total\",\"Call Premium Total\"");
        }

        write.WriteLine(content);
    }

    private static string GetCsvLine(Parser.ParserResult result)
    {
        var title = result.PageTitle;
        var totals = result.Totals;

        return $"\"{title?.TradeTime}\",\"{DateTime.Now:HH:mm:ss}\",\"{title?.LastPrice}\",\"{totals?.PutPremiumTotal}\",\"{totals?.CallPremiumTotal}\"";
    }
}
