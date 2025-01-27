using System.Text.Json.Serialization;

namespace FuturesScrapper.Models;

/*
"symbol": "NGG25",
"symbolName": "Natural Gas",
"symbolType": 2,
"lastPrice": "3.354s",
"priceChange": "-0.306",
"percentChange": "-8.36%",
"bidPrice": "N\/A",
"askPrice": "N\/A",
"bidSize": "N\/A",
"askSize": "N\/A",
"tradeTime": "Friday January 3rd",
"lastPriceExt": "-",
"priceChangeExt": "-",
"percentChangeExt": "-",
"tradeTimeExt": "-",
"contractName": "Natural Gas (Feb \u002725)",
"daysToContractExpiration": "24",
"symbolCode": "FUT",
"exchange": "NYMEX",
"sicIndustry": "N\/A",
"symbolRoot": "NG",
"sessionDateDisplayLong": "Fri, Jan 3rd, 2025",
"shouldUpdate": true
*/


public record PageTitle
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; init; } = string.Empty;

    [JsonPropertyName("symbolName")]
    public string SymbolName { get; init; }= string.Empty;

    [JsonPropertyName("lastPrice")]
    public string LastPrice { get; init; }= string.Empty;

    [JsonPropertyName("priceChange")]
    public string PriceChange { get; init; }= string.Empty;

    [JsonPropertyName("percentChange")]
    public string PercentChange { get; init; }= string.Empty;

    [JsonPropertyName("tradeTime")]
    public string TradeTime { get; init; }= string.Empty;
}
