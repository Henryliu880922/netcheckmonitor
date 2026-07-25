using System.IO;
using NetCheckMonitor.Core;
using NetCheckMonitor.Core.Report;
using NetCheckMonitor.Core.Report.Csv;

namespace NetCheckMonitor.Cli.Commands;

internal static class ReportCommand
{
    public static void Execute(string[] args)
    {
        ReportArguments arguments;

        try
        {
            arguments = ReportArgumentsParser.Parse(args);
        }
        catch (ArgumentException)
        {
            Console.WriteLine("Usage:");
            Console.WriteLine(
                "  netcheckmonitor report --input <monitor.csv> --output <report.csv>");
            return;
        }

        string inputPath = arguments.InputPath;
        string outputPath = arguments.OutputPath;

        if (!File.Exists(inputPath))
        {
            Console.Error.WriteLine($"Input file not found: {inputPath}");
            return;
        }

        var runner = new ReportRunner();

        runner.Run(inputPath, outputPath);

        Console.WriteLine($"Report written to: {outputPath}");
    }
}