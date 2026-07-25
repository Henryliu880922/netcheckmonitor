using System.IO;
using NetCheckMonitor.Core;
using NetCheckMonitor.Core.Report;
using NetCheckMonitor.Core.Report.Csv;

namespace NetCheckMonitor.Cli.Commands;

internal static class ReportCommand
{
    public static int Execute(string[] args)
    {
        if (args.Length == 1 &&
    args[0].Equals("--help", StringComparison.OrdinalIgnoreCase))
        {
            HelpCommand.ShowReportHelp();
            return 0;
        }
        ReportArguments arguments;

        try
        {
            arguments = ReportArgumentsParser.Parse(args);
        }
        catch (ArgumentException)
        {
            HelpCommand.ShowReportHelp();
            return 1;
        }

        string inputPath = arguments.InputPath;
        string outputPath = arguments.OutputPath;

        if (!File.Exists(inputPath))
        {
            Console.Error.WriteLine($"Input file not found: {inputPath}");
            return 2;
        }

        var runner = new ReportRunner();

        runner.Run(inputPath, outputPath);

        Console.WriteLine($"Report written to: {outputPath}");
        return 0;
    }
}