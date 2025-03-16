namespace FuturesScrapper;

public sealed class ScraperService(ILogger<IScraperService> logger, HttpClient httpClient) : IScraperService
{
    public async Task ExecuteAsync(Uri url, string csvFile, bool overwrite, CancellationToken stoppingToken)
    {
        var content = await GetUrlContentAsync(url, stoppingToken);

        if (string.IsNullOrEmpty(content))
        {
            logger.LogError("No content");
            return;
        }

        var result = Parser.Parse(content);

        if (result == null)
        {
            logger.LogError("Could not parse HTML");
            return;
        }

        string newLineEntry = GetCsvLine(result);

        if (overwrite && ShouldOverwrite(csvFile, newLineEntry))
        {
            OverwriteCsv(csvFile, newLineEntry);
            logger.LogInformation("Over-written last entry as 'Put Premium Total' and 'Call Premium Total' were same");
        }
        else
        {
            WriteCsv(csvFile, newLineEntry);
        }
    }

    private bool ShouldOverwrite(string csvFile, ReadOnlySpan<char> newLineEntry)
    {
        var lines = File.ReadLines(csvFile);
        var count = lines.Count();

        if (count == 0)
        {
            return false;
        }

        var lastLine = lines.Last();

        if (string.IsNullOrEmpty(lastLine))
        {
            return false;
        }

        string[] cells = lastLine.Split(',');

        if (cells.Length != 5)
        {
            logger.LogError("List line is not in proper format");
            return false;
        }

        if (newLineEntry.EndsWith($"{cells[3]},{cells[4]}") == false)
        {
            return false;
        }

        return true;
    }

    private async Task<string?> GetUrlContentAsync(Uri url, CancellationToken stoppingToken)
    {
        try
        {
            using var response = await httpClient.GetAsync(url, stoppingToken);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(stoppingToken);

            return content;
        }
        catch (Exception error)
        {
            logger.LogError("{}", error.Message);
            return null;
        }
    }

    private void WriteCsv(string csvFile, string content)
    {

        if (string.IsNullOrEmpty(csvFile))
        {
            logger.LogCritical("Cannot write CSV as path is empty");
            return;
        }

        bool empty = false;

        if (!File.Exists(csvFile))
        {
            logger.LogInformation("File '{}' doesn't exist, creating new one", csvFile);

            var dir = Path.GetDirectoryName(csvFile);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            empty = true;
        }

        using var writer = File.AppendText(csvFile);

        if (empty)
        {
            writer.WriteLine("\"Trade Time\",\"TimeStamp\",\"Last Price\",\"Put Premium Total\",\"Call Premium Total\"");
        }

        writer.WriteLine(content);

        logger.LogInformation("[{}] A new entry added to {}", DateTime.Now, Path.GetFileName(csvFile));
    }

    private static void OverwriteCsv(string csvFile, ReadOnlySpan<char> newLineEntry)
    {
        var tmpFile = Path.GetTempFileName();
        var lines = File.ReadLines(csvFile);
        var count = lines.Count();
        using var writer = File.AppendText(tmpFile);

        int i = 0;
        foreach (var line in lines)
        {
            i++;
            writer.WriteLine(i == count ? newLineEntry : line);
        }

        File.Replace(tmpFile, csvFile, null);
    }

    private static string GetCsvLine(Parser.ParserResult result)
    {
        var title = result.PageTitle;
        var totals = result.Totals;

        return $"\"{title?.TradeTime}\",\"{DateTime.Now:HH:mm:ss}\",\"{title?.LastPrice}\",\"{totals?.PutPremiumTotal.Replace(",", string.Empty)}\",\"{totals?.CallPremiumTotal.Replace(",", string.Empty)}\"";
    }
}
