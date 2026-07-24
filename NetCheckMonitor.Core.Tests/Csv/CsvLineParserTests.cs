using NetCheckMonitor.Core.Report.Csv;

namespace NetCheckMonitor.Core.Tests;

public sealed class CsvLineParserTests
{
    [Fact]
    public void Parse_SimpleFields_ReturnsAllValues()
    {
        IReadOnlyList<string> fields =
            CsvLineParser.Parse("2026-07-23T21:00:00,CHECK,ONLINE,12,https://example.com/,OK");

        Assert.Equal(6, fields.Count);
        Assert.Equal("CHECK", fields[1]);
        Assert.Equal("ONLINE", fields[2]);
        Assert.Equal("12", fields[3]);
    }

    [Fact]
    public void Parse_QuotedComma_KeepsCommaInsideField()
    {
        IReadOnlyList<string> fields =
            CsvLineParser.Parse(
                "2026-07-23T21:00:00,MARKER,EVENT_NOTE,,,\"重新啟動路由器, 已恢復\"");

        Assert.Equal(6, fields.Count);
        Assert.Equal("重新啟動路由器, 已恢復", fields[5]);
    }

    [Fact]
    public void Parse_EscapedQuote_ReturnsSingleQuoteCharacter()
    {
        IReadOnlyList<string> fields =
            CsvLineParser.Parse(
                "2026-07-23T21:00:00,MARKER,EVENT_NOTE,,,\"使用者說 \"\"已恢復\"\"\"");

        Assert.Equal("使用者說 \"已恢復\"", fields[5]);
    }
}