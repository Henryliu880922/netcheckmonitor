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

            string[] lines = output
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            Assert.Equal(2, lines.Length);

            Assert.StartsWith("Version ", lines[1]);
            Assert.True(lines[1].Length > "Version ".Length);
        }
        finally
        {
            Console.SetOut(originalOutput);
        }
    }
}