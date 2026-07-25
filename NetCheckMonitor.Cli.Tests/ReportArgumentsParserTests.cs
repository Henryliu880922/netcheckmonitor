using NetCheckMonitor.Cli.Commands;
using Xunit;

namespace NetCheckMonitor.Cli.Tests;

public class ReportArgumentsParserTests
{
    [Fact]
    public void Parse_ValidArguments_ReturnsArguments()
    {
        string[] args =
        [
            "--input",
            "monitor.csv",
            "--output",
            "report.csv"
        ];

        ReportArguments result =
            ReportArgumentsParser.Parse(args);

        Assert.Equal("monitor.csv", result.InputPath);
        Assert.Equal("report.csv", result.OutputPath);
    }
    [Fact]
    public void Parse_MissingInput_ThrowsArgumentException()
    {
        string[] args =
        [
            "--output",
        "report.csv"
        ];

        Assert.Throws<ArgumentException>(
            () => ReportArgumentsParser.Parse(args));
    }
    [Fact]
    public void Parse_MissingOutput_ThrowsArgumentException()
    {
        string[] args =
        [
            "--input",
        "monitor.csv"
        ];

        Assert.Throws<ArgumentException>(
            () => ReportArgumentsParser.Parse(args));
    }
    [Fact]
    public void Parse_InvalidOption_ThrowsArgumentException()
    {
        string[] args =
        [
            "--in",
        "monitor.csv",
        "--output",
        "report.csv"
        ];

        Assert.Throws<ArgumentException>(
            () => ReportArgumentsParser.Parse(args));
    }
}