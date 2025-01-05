using System.Text.Json;
using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using FuturesScapper.Models;

namespace FuturesScapper;

public partial class Parser
{
    public record ParserResult
    {
        public PageTitle? PageTitle { get; set; }
        public Totals? Totals { get; set; }
    }

    public static ParserResult? Parse(string source)
    {
        var context = BrowsingContext.New(Configuration.Default);
        var parser = context.GetService<IHtmlParser>();
        var document = parser?.ParseDocument(source);

        if (document == null)
        {
            return null;
        }

        return new ParserResult
        {
            PageTitle = GetPageTitle(document),
            Totals = GetTotals(document)
        };
    }

    private static PageTitle? GetPageTitle(IHtmlDocument document)
    {
        var attrib = document.QuerySelector(".page-title")?.GetAttribute("data-ng-init");

        if (string.IsNullOrEmpty(attrib) || attrib.Length < 10)
        {
            return null;
        }

        var json = attrib.AsSpan().Slice(5, attrib.Length - 6);

        return JsonSerializer.Deserialize<PageTitle>(json);
    }

    [GeneratedRegex("[\r\n ]+")]
    private static partial Regex WhiteSpaceRegex();

    private static Totals? GetTotals(IHtmlDocument document)
    {
        var nodes = document.QuerySelectorAll(".bc-futures-options-quotes-totals__data-row");

        if (nodes == null)
        {
            return null;
        }

        var totals = new Totals();

        foreach (var node in nodes)
        {
            var text = WhiteSpaceRegex().Replace(node.TextContent, " ").AsSpan().Trim();

            if (text.StartsWith(Totals.CALL_OPEN_INTEREST_TOTAL))
            {
                totals.CallOpenInterestTotal = text[Totals.CALL_OPEN_INTEREST_TOTAL.Length..].Trim().ToString();
                continue;
            }

            if (text.StartsWith(Totals.CALL_PREMIUM_TOTAL))
            {
                totals.CallPremiumTotal = text[Totals.CALL_PREMIUM_TOTAL.Length..].Trim().ToString();
                continue;
            }

            if (text.StartsWith(Totals.PUT_OPEN_INTEREST_TOTAL))
            {
                totals.PutOpenInterestTotal = text[Totals.PUT_OPEN_INTEREST_TOTAL.Length..].Trim().ToString();
                continue;
            }

            if (text.StartsWith(Totals.PUT_CALL_OPEN_INTEREST_RATIO))
            {
                totals.PutCallOpenInterestRatio = text[Totals.PUT_CALL_OPEN_INTEREST_RATIO.Length..].Trim().ToString();
                continue;
            }

            if (text.StartsWith(Totals.PUT_CALL_PREMIUM_RATIO))
            {
                totals.PutCallPremiumRatio = text[Totals.PUT_CALL_PREMIUM_RATIO.Length..].Trim().ToString();
                continue;
            }

            if (text.StartsWith(Totals.PUT_PREMIUM_TOTAL))
            {
                totals.PutPremiumTotal = text[Totals.PUT_PREMIUM_TOTAL.Length..].Trim().ToString();
                continue;
            }
        }

        return totals;

    }
}