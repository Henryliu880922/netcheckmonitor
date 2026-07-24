using System.IO;
using NetCheckMonitor.Core.Report;
using NetCheckMonitor.Core.Report.Csv;
using NetCheckMonitor.Core;

namespace NetCheckMonitor.Cli.Commands;

internal static class ReportCommand
{
    public static void Execute(string[] args)
    {
        if (args.Length != 4 ||
            !args[0].Equals("--input", StringComparison.OrdinalIgnoreCase) ||
            !args[2].Equals("--output", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Usage:");
            Console.WriteLine(
                "  netcheckmonitor report --input <monitor.csv> --output <report.csv>");
            return;
        }

        string inputPath = args[1];
        string outputPath = args[3];

        if (!File.Exists(inputPath))
        {
            Console.WriteLine($"Input file not found: {inputPath}");
            return;
        }

        var parser = new CsvSessionParser();
        MonitoringSession session = parser.Parse(inputPath);

        var reportService = new ReportService();

        string csv = reportService.ExportDailyReportCsv(session);

        File.WriteAllText(outputPath, csv);

        Console.WriteLine($"Report written to: {outputPath}");
    }
}