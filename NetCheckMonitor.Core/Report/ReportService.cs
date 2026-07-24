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
}