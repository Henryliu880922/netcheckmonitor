using System;
using System.IO;
using NetCheckMonitor.Cli.Commands;
using Xunit;

public class ReportCommandTests
{
    [Fact]
    public void Execute_InputFileDoesNotExist_ReturnsExitCode2()
    {
        TextWriter originalError = Console.Error;
        var writer = new StringWriter();

        try
        {
            Console.SetError(writer);

            int exitCode = ReportCommand.Execute(
                [
                    "--input", "missing.csv",
                    "--output", "report.csv"
                ]);

            Assert.Equal(2, exitCode);

            string output = writer.ToString();

            Assert.Contains("Input file not found", output);
            Assert.Contains("missing.csv", output);
        }
        finally
        {
            Console.SetError(originalError);
        }
    }
    [Fact]
    public void Execute_MissingRequiredArguments_ReturnsExitCode1()
    {
        TextWriter originalOutput = Console.Out;
        var writer = new StringWriter();

        try
        {
            Console.SetOut(writer);

            int exitCode = ReportCommand.Execute(
                [
                    "--input", "monitor.csv"
                ]);

            Assert.Equal(1, exitCode);

            string output = writer.ToString();

            Assert.Contains("Usage:", output);
            Assert.Contains(
                "netcheckmonitor report --input <monitor.csv> --output <report.csv>",
                output);
        }
        finally
        {
            Console.SetOut(originalOutput);
        }
    }
}