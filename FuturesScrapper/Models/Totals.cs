namespace FuturesScrapper.Models;

public record Totals
{
    public const string PUT_PREMIUM_TOTAL = "Put Premium Total";
    public const string CALL_PREMIUM_TOTAL = "Call Premium Total";
    public const string PUT_CALL_PREMIUM_RATIO = "Put/Call Premium Ratio";
    public const string PUT_OPEN_INTEREST_TOTAL = "Put Open Interest Total";
    public const string CALL_OPEN_INTEREST_TOTAL = "Call Open Interest Total";
    public const string PUT_CALL_OPEN_INTEREST_RATIO = "Put/Call Open Interest Ratio";
    
    public string PutPremiumTotal { get; set; } = string.Empty;
    public string CallPremiumTotal { get; set; } = string.Empty;
    public string PutCallPremiumRatio { get; set; } = string.Empty;
    public string PutOpenInterestTotal { get; set; } = string.Empty;
    public string CallOpenInterestTotal { get; set; } = string.Empty;
    public string PutCallOpenInterestRatio { get; set; } = string.Empty;
}
