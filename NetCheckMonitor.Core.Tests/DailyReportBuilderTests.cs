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
}