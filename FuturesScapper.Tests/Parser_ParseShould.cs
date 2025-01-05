using FuturesScapper.Models;

namespace FuturesScapper.Tests;

public class Parser_ParseShould
{
    [Fact]
    public async Task ParsePageTitleAsync()
    {
        var content = await File.ReadAllTextAsync("data/page.html");
        var result = Parser.Parse(content);
        var expectedPageTitle = new PageTitle()
        {
            Symbol = "NGG25",
            SymbolName = "Natural Gas",
            LastPrice = "3.354s",
            PriceChange = "-0.306",
            PercentChange = "-8.36%",
            TradeTime = "Friday January 3rd"
        };

        Assert.Equal(expectedPageTitle, result?.PageTitle);
    }

    [Fact]
    public async Task ParseTotalsAsync()
    {
        var content = await File.ReadAllTextAsync("data/page.html");
        var result = Parser.Parse(content);
        var expectedTotals = new Totals
        {
            PutPremiumTotal = "$879,260.00",
            CallPremiumTotal = "$231,220.00",
            PutCallPremiumRatio = "3.80",
            PutOpenInterestTotal = "22,831",
            CallOpenInterestTotal = "30,235",
            PutCallOpenInterestRatio = "0.76"
        };

        Assert.Equal(expectedTotals, result?.Totals);
    }
}