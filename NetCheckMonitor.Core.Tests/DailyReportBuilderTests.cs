using NetCheckMonitor.Core.Report;
using Xunit;

namespace NetCheckMonitor.Core.Tests;

public class DailyReportBuilderTests
{
    [Fact]
    public void Build_CopiesBasicInformation()
    {
        var session = new MonitoringSession
        {
            MachineName = "PC01",
            MachineId = "ABC123",
            Start = new DateTime(2026, 7, 24, 8, 0, 0, DateTimeKind.Utc)
        };

        session.Records.Add(new MonitoringRecord());
        session.EventNotes.Add(new EventNote());

        var builder = new DailyReportBuilder();

        var report = builder.Build(session);

        Assert.Equal("PC01", report.MachineName);
        Assert.Equal("ABC123", report.MachineId);
        Assert.Equal(new DateTime(2026, 7, 24), report.Day);
        Assert.Single(report.Records);
        Assert.Single(report.EventNotes);
    }
    [Fact]
    public void Build_CalculatesEffectiveTime()
    {
        var session = new MonitoringSession
        {
            Start = new DateTime(2026, 7, 24, 9, 0, 0, DateTimeKind.Utc),
            End = new DateTime(2026, 7, 24, 10, 0, 0, DateTimeKind.Utc),
            Stopped = true
        };

        session.PausePeriods.Add(new PausePeriod
        {
            Start = new DateTime(2026, 7, 24, 9, 10, 0, DateTimeKind.Utc),
            End = new DateTime(2026, 7, 24, 9, 20, 0, DateTimeKind.Utc)
        });

        var builder = new DailyReportBuilder();

        var report = builder.Build(session);

        Assert.Equal(
            TimeSpan.FromMinutes(50),
            report.Effective);
    }
    [Fact]
    public void Build_CalculatesTotalOutageTime()
    {
        var session = new MonitoringSession
        {
            Start = new DateTime(2026, 7, 24, 9, 0, 0, DateTimeKind.Utc),
            End = new DateTime(2026, 7, 24, 10, 0, 0, DateTimeKind.Utc),
            Stopped = true
        };

        session.Outages.Add(new OutagePeriod
        {
            Start = new DateTime(2026, 7, 24, 9, 10, 0, DateTimeKind.Utc),
            End = new DateTime(2026, 7, 24, 9, 13, 0, DateTimeKind.Utc)
        });

        session.Outages.Add(new OutagePeriod
        {
            Start = new DateTime(2026, 7, 24, 9, 30, 0, DateTimeKind.Utc),
            End = new DateTime(2026, 7, 24, 9, 35, 0, DateTimeKind.Utc)
        });

        var builder = new DailyReportBuilder();

        var report = builder.Build(session);

        Assert.Equal(
            TimeSpan.FromMinutes(8),
            report.Outage);
    }
    [Fact]
    public void Build_CalculatesLongestOutage()
    {
        var session = new MonitoringSession
        {
            Start = new DateTime(2026, 7, 24, 9, 0, 0, DateTimeKind.Utc),
            End = new DateTime(2026, 7, 24, 10, 0, 0, DateTimeKind.Utc),
            Stopped = true
        };

        session.Outages.Add(new OutagePeriod
        {
            Start = new DateTime(2026, 7, 24, 9, 10, 0, DateTimeKind.Utc),
            End = new DateTime(2026, 7, 24, 9, 13, 0, DateTimeKind.Utc)
        });

        session.Outages.Add(new OutagePeriod
        {
            Start = new DateTime(2026, 7, 24, 9, 30, 0, DateTimeKind.Utc),
            End = new DateTime(2026, 7, 24, 9, 35, 0, DateTimeKind.Utc)
        });

        var builder = new DailyReportBuilder();

        var report = builder.Build(session);

        Assert.Equal(
            TimeSpan.FromMinutes(5),
            report.LongestOutage);
    }
}