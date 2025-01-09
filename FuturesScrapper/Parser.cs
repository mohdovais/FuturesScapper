using System.Text.Json;
using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using FuturesScrapper.Models;

namespace FuturesScrapper;

public partial class Parser
{
    public record ParserResult
    {
        public PageTitle? PageTitle { get; init; }
        public Totals? Totals { get; init; }
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

        return JsonSerializer.Deserialize(json, SourceGenerationContext.Default.PageTitle);
    }

    [GeneratedRegex("[\r\n ]+")]
    private static partial Regex WhiteSpaceRegex();

    private static Totals? GetTotals(IHtmlDocument document)
    {
        var nodes = document.QuerySelectorAll(".bc-futures-options-quotes-totals__data-row");

        if (nodes.Length == 0)
        {
            return null;
        }

        var totals = new Totals();

        foreach (var node in nodes)
        {
            var text = WhiteSpaceRegex().Replace(node.TextContent, " ").AsSpan().Trim();

            if (text.StartsWith(Totals.CALL_OPEN_INTEREST_TOTAL))
            {
                totals.CallOpenInterestTotal = GetValue(text, Totals.CALL_OPEN_INTEREST_TOTAL.Length);
                continue;
            }

            if (text.StartsWith(Totals.CALL_PREMIUM_TOTAL))
            {
                totals.CallPremiumTotal = GetValue(text, Totals.CALL_PREMIUM_TOTAL.Length);
                continue;
            }

            if (text.StartsWith(Totals.PUT_OPEN_INTEREST_TOTAL))
            {
                totals.PutOpenInterestTotal = GetValue(text, Totals.PUT_OPEN_INTEREST_TOTAL.Length);
                continue;
            }

            if (text.StartsWith(Totals.PUT_CALL_OPEN_INTEREST_RATIO))
            {
                totals.PutCallOpenInterestRatio = GetValue(text, Totals.PUT_CALL_OPEN_INTEREST_RATIO.Length);
                continue;
            }

            if (text.StartsWith(Totals.PUT_CALL_PREMIUM_RATIO))
            {
                totals.PutCallPremiumRatio = GetValue(text, Totals.PUT_CALL_PREMIUM_RATIO.Length);
                continue;
            }

            if (text.StartsWith(Totals.PUT_PREMIUM_TOTAL))
            {
                totals.PutPremiumTotal = GetValue(text, Totals.PUT_PREMIUM_TOTAL.Length);
            }
        }

        return totals;

    }

    private static string GetValue(ReadOnlySpan<char> text, int startIndex)
    {
        var span = text[startIndex..].Trim();
        if (span.StartsWith('$'))
        {
            span = span[1..];
        }
        return span.ToString();
    }
}
