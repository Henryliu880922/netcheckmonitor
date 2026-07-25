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
    [Fact]
    public void Execute_ValidArguments_CreatesReportAndReturnsExitCode0()
    {
        string tempDirectory = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString());

        Directory.CreateDirectory(tempDirectory);

        try
        {
            string inputPath = Path.Combine(tempDirectory, "monitor.csv");
            string outputPath = Path.Combine(tempDirectory, "report.csv");

            File.WriteAllText(
                inputPath,
                """
            Timestamp,Status,Latency
            2025-01-01T00:00:00,Online,20
            2025-01-01T00:01:00,Online,25
            """);

            int exitCode = ReportCommand.Execute(
            [
                "--input", inputPath,
            "--output", outputPath
            ]);

            Assert.Equal(0, exitCode);

            Assert.True(File.Exists(outputPath));

            string report = File.ReadAllText(outputPath);

            Assert.False(string.IsNullOrWhiteSpace(report));

            Assert.Contains("Day", report);

            Assert.Contains("Availability", report);
        }
        finally
        {
            Directory.Delete(tempDirectory, true);
        }
    }
}