using NetCheckMonitor.Core.Report.Csv;

namespace NetCheckMonitor.Core.Tests;

public class CsvSessionParserTests
{
    [Fact]
    public void Parse_EmptyFile_ReturnsEmptySession()
    {
        var parser = new CsvSessionParser();

        var session = parser.Parse(Array.Empty<string>());

        Assert.NotNull(session);
        Assert.Empty(session.Records);
    }
    [Fact]
    public void Parse_CheckRecord_AddsMonitoringRecord()
    {
    var parser = new CsvSessionParser();

    var session = parser.Parse(new[]
    {
        "Timestamp,Type,Status,LatencyMs,Target,Detail",
        "2026-07-23T21:00:00Z,CHECK,ONLINE,12,https://example.com/,OK"
    });

    MonitoringRecord record = Assert.Single(session.Records);

    Assert.True(record.Online);
    Assert.Equal("ONLINE", record.Status);
    Assert.Equal(12, record.Latency);
    Assert.Equal("https://example.com/", record.Target);
    Assert.Equal("OK", record.Detail);
    }
    [Fact]
    public void Parse_StartAndStopMarkers_SetSessionTimes()
    {
    var parser = new CsvSessionParser();
    var session = parser.Parse(new[]
    {
        "Timestamp,Type,Status,LatencyMs,Target,Detail",
        "2026-07-23T20:00:00Z,MARKER,STARTED,,,,",
        "2026-07-23T20:30:00Z,CHECK,ONLINE,15,https://example.com/,OK",
        "2026-07-23T21:00:00Z,MARKER,STOPPED,,,,"
    });
    Assert.Equal(
        new DateTime(2026, 7, 23, 20, 0, 0, DateTimeKind.Utc),
        session.Start);
    Assert.Equal(
        new DateTime(2026, 7, 23, 21, 0, 0, DateTimeKind.Utc),
        session.End);
    Assert.True(session.Stopped);
}
    [Fact]
    public void Parse_EventNote_AddsEventNote()
    {
    var parser = new CsvSessionParser();
    var session = parser.Parse(new[]
    {
        "Timestamp,Type,Status,LatencyMs,Target,Detail",
        "2026-07-23T20:00:00Z,MARKER,EVENT_NOTE,,,Restarted router"
    });
    var note = Assert.Single(session.EventNotes);
    Assert.Equal(
        new DateTime(2026, 7, 23, 20, 0, 0, DateTimeKind.Utc),
        note.Time);
    Assert.Equal("Restarted router", note.Text);
}
    [Fact]
    public void Parse_ComputerMarker_SetsMachineInformation()
    {
    var parser = new CsvSessionParser();
    var session = parser.Parse(new[]
    {
        "Timestamp,Type,Status,LatencyMs,Target,Detail",
        "2026-07-23T20:00:00Z,MARKER,COMPUTER,,,MacBook-Pro [ABC123]"
    });
    Assert.Equal("MacBook-Pro", session.MachineName);
    Assert.Equal("ABC123", session.MachineId);
}
}