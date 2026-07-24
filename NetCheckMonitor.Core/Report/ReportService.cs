using System.Globalization;

namespace NetCheckMonitor.Core.Report;

public sealed class ReportService
{
    private readonly DailyReportBuilder _builder = new();

    public DailyReportData BuildDailyReport(
        MonitoringSession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        return _builder.Build(session);
    }

    public string ExportDailyReportCsv(
        MonitoringSession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        DailyReportData report = _builder.Build(session);

        const string header =
            "Day,MachineName,MachineId,Availability,CheckCount,OfflineCount,AverageLatency,MaximumLatency";

        string row = string.Join(",",
            report.Day.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            report.MachineName,
            report.MachineId,
            report.Availability.ToString(
                CultureInfo.InvariantCulture),
            report.CheckCount.ToString(
                CultureInfo.InvariantCulture),
            report.OfflineCount.ToString(
                CultureInfo.InvariantCulture),
            report.AverageLatency.ToString(
                CultureInfo.InvariantCulture),
            report.MaximumLatency.ToString(
                CultureInfo.InvariantCulture));

        return $"{header}\n{row}";
    }
}