using Xunit;
using System.IO;
using NetCheckMonitor.Cli;

namespace NetCheckMonitor.Cli.Tests;

public class ReportRunnerTests
{
    [Fact]
    public void Run_CreatesOutputFile()
    {
        string input = Path.GetTempFileName();

        string output = Path.GetTempFileName();

        var runner = new ReportRunner();

        runner.Run(input, output);

        Assert.True(File.Exists(output));

        string content = File.ReadAllText(output);

        Assert.False(string.IsNullOrWhiteSpace(content));
    }
}