using NetCheckMonitor.Core;
using NetCheckMonitor.Core.Report;
using NetCheckMonitor.Core.Report.Csv;

namespace NetCheckMonitor.Cli;

internal sealed class ReportRunner
{
    public void Run(string inputPath, string outputPath)
    {
        var parser = new CsvSessionParser();

        MonitoringSession session = parser.Parse(inputPath);

        var reportService = new ReportService();

        string csv = reportService.ExportDailyReportCsv(session);

        File.WriteAllText(outputPath, csv);
    }
}