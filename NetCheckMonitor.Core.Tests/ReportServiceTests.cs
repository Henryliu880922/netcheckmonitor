using NetCheckMonitor.Core.Report;

namespace NetCheckMonitor.Core.Tests;

public sealed class ReportServiceTests
{
    [Fact]
    public void BuildDailyReport_ReturnsReportForSession()
    {
        var session = new MonitoringSession
        {
            MachineName = "Test-Mac",
            MachineId = "MAC-001",
            Start = new DateTime(
                2026, 7, 24, 9, 0, 0,
                DateTimeKind.Utc),
            End = new DateTime(
                2026, 7, 24, 10, 0, 0,
                DateTimeKind.Utc),
            Stopped = true
        };

        var service = new ReportService();

        var report = service.BuildDailyReport(session);

        Assert.Equal("Test-Mac", report.MachineName);
        Assert.Equal("MAC-001", report.MachineId);
        Assert.Equal(session.Start.Date, report.Day);
    }
    [Fact]
    public void ExportDailyReportCsv_ReturnsHeaderAndReportData()
    {
        var session = new MonitoringSession
        {
            MachineName = "Test-Mac",
            MachineId = "MAC-001",
            Start = new DateTime(
                2026, 7, 24, 9, 0, 0,
                DateTimeKind.Utc),
            End = new DateTime(
                2026, 7, 24, 10, 0, 0,
                DateTimeKind.Utc),
            Stopped = true
        };

        var service = new ReportService();

        var csv = service.ExportDailyReportCsv(session);

        var expected =
            "Day,MachineName,MachineId,Availability,CheckCount,OfflineCount,AverageLatency,MaximumLatency\n" +
            "2026-07-24,Test-Mac,MAC-001,100,0,0,0,0";

        Assert.Equal(expected, csv);
    }
}