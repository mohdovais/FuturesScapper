namespace FuturesScrapper.Models;

public record Settings
{
    public const string Name = "Settings";

    public Uri? Url { get; set; }
    public string? CsvFile { get; set; }
    public int DelayInMinutes { get; set; } = 1;
}
