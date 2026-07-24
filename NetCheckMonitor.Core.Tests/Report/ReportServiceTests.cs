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
    [Fact]
    public void ExportDailyReportCsv_QuotesFieldsContainingComma()
    {
        var session = new MonitoringSession
        {
            MachineName = "MacBook, Pro",
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

        Assert.Contains("\"MacBook, Pro\"", csv);
    }
    [Fact]
    public void ExportDailyReportCsv_QuotesFieldsContainingQuotes()
    {
        var session = new MonitoringSession
        {
            MachineName = "Henry \"MIS\"",
            MachineId = "MAC-001",
            Start = new DateTime(2026, 7, 24),
            End = new DateTime(2026, 7, 24, 1, 0, 0),
            Stopped = true
        };

        var service = new ReportService();

        var csv = service.ExportDailyReportCsv(session);

        Assert.Contains("\"Henry \"\"MIS\"\"\"", csv);
    }
    [Fact]
    public void ExportDailyReportCsv_QuotesFieldsContainingNewLine()
    {
        var session = new MonitoringSession
        {
            MachineName = "MacBook\nPro",
            MachineId = "MAC-001",
            Start = new DateTime(2026, 7, 24),
            End = new DateTime(2026, 7, 24, 1, 0, 0),
            Stopped = true
        };

        var service = new ReportService();

        var csv = service.ExportDailyReportCsv(session);

        Assert.Contains("\"MacBook\nPro\"", csv);
    }
    [Fact]
    public void ExportDailyReportCsvFile_WritesCsvFile()
    {
        string path = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid() + ".csv");

        try
        {
            var session = new MonitoringSession
            {
                MachineName = "Test-Mac",
                MachineId = "MAC-001",
                Start = new DateTime(2026, 7, 24),
                End = new DateTime(2026, 7, 24, 1, 0, 0),
                Stopped = true
            };

            var service = new ReportService();

            service.ExportDailyReportCsvFile(session, path);

            Assert.True(File.Exists(path));

            string csv = File.ReadAllText(path);

            Assert.Contains("MachineName", csv);
            Assert.Contains("Test-Mac", csv);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
    [Fact]
    public void ExportDailyReportCsvFile_Throws_WhenSessionIsNull()
    {
        var service = new ReportService();

        Assert.Throws<ArgumentNullException>(() =>
            service.ExportDailyReportCsvFile(
                null!,
                "report.csv"));
    }
    [Fact]
    public void ExportDailyReportCsvFile_Throws_WhenPathIsNull()
    {
        var session = new MonitoringSession
        {
            Start = DateTime.UtcNow,
            End = DateTime.UtcNow,
            Stopped = true
        };

        var service = new ReportService();

        Assert.Throws<ArgumentNullException>(() =>
            service.ExportDailyReportCsvFile(
                session,
                null!));
    }
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ExportDailyReportCsvFile_Throws_WhenPathIsEmptyOrWhiteSpace(
    string path)
    {
        var session = new MonitoringSession
        {
            Start = DateTime.UtcNow,
            End = DateTime.UtcNow,
            Stopped = true
        };

        var service = new ReportService();

        Assert.Throws<ArgumentException>(() =>
            service.ExportDailyReportCsvFile(
                session,
                path));
    }
}