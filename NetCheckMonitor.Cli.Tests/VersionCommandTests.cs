using System;
using System.IO;
using NetCheckMonitor.Cli.Commands;
using Xunit;

public class VersionCommandTests
{
    [Fact]
    public void Execute_WritesVersionInformation()
    {
        TextWriter originalOutput = Console.Out;
        var writer = new StringWriter();

        try
        {
            Console.SetOut(writer);

            VersionCommand.Execute();

            string output = writer.ToString();

            Assert.Contains("NetCheckMonitor CLI", output);
            Assert.Contains("Version", output);
        }
        finally
        {
            Console.SetOut(originalOutput);
        }
    }
}