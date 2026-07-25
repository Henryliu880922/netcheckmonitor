using System;
using System.IO;
using NetCheckMonitor.Cli.Commands;
using Xunit;

public class HelpCommandTests
{
    [Fact]
    public void ShowGeneralHelp_WritesUsage()
    {
        var writer = new StringWriter();
        Console.SetOut(writer);

        HelpCommand.ShowGeneralHelp();

        string output = writer.ToString();

        Assert.Contains("NetCheckMonitor CLI", output);
        Assert.Contains("Usage:", output);
        Assert.Contains("report", output);
    }
}