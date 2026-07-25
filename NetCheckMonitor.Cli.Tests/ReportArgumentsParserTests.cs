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
}