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
    [Fact]
    public void Parse_NetworkMarker_AddsNetworkInfo()
    {
        var parser = new CsvSessionParser();
        var session = parser.Parse(new[]
        {
        "Timestamp,Type,Status,LatencyMs,Target,Detail",
        "2026-07-23T20:00:00Z,MARKER,NETWORK,,,Adapter=Wi-Fi;Description=Intel AX210;Type=Wireless"
    });
        var network = Assert.Single(session.Networks);
        Assert.Equal("Wi-Fi", network.Adapter);
        Assert.Equal("Intel AX210", network.Description);
        Assert.Equal("Wireless", network.Type);
    }
    [Fact]
    public void Parse_NetworkMarker_ParsesWirelessDetails()
    {
        var parser = new CsvSessionParser();
        var session = parser.Parse(new[]
        {
        "Timestamp,Type,Status,LatencyMs,Target,Detail",
        "2026-07-23T20:00:00Z,MARKER,NETWORK,,,Adapter=Wi-Fi;Description=Intel AX210;Type=Wireless;SSID=OfficeWiFi;BSSID=AA:BB:CC:DD:EE:FF;Signal=87"
    });
        var network = Assert.Single(session.Networks);
        Assert.Equal("OfficeWiFi", network.SSID);
        Assert.Equal("AA:BB:CC:DD:EE:FF", network.BSSID);
        Assert.Equal(87, network.Signal);
    }
    [Fact]
    public void Parse_NetworkMarker_ParsesIpDetails()
    {
        var parser = new CsvSessionParser();
        var session = parser.Parse(new[]
        {
        "Timestamp,Type,Status,LatencyMs,Target,Detail",
        "2026-07-24T08:00:00Z,MARKER,NETWORK,,,Adapter=Wi-Fi;Description=Intel AX210;Type=Wireless;IPv4=192.168.1.50;IPv6=2001:db8::50;Gateway=192.168.1.1;DNS=1.1.1.1"
    });
        var network = Assert.Single(session.Networks);
        Assert.Equal("192.168.1.50", network.IPv4);
        Assert.Equal("2001:db8::50", network.IPv6);
        Assert.Equal("192.168.1.1", network.Gateway);
        Assert.Equal("1.1.1.1", network.Dns);
    }
    [Fact]
    public void Parse_NetworkMarker_ParsesMacAddress()
    {
        var parser = new CsvSessionParser();

        var session = parser.Parse(new[]
        {
        "Timestamp,Type,Status,LatencyMs,Target,Detail",
        "2026-07-24T08:30:00Z,MARKER,NETWORK,,,Adapter=Ethernet;Description=USB LAN;Type=Wired;MAC=AA:BB:CC:DD:EE:FF"
    });

        var network = Assert.Single(session.Networks);

        Assert.Equal("AA:BB:CC:DD:EE:FF", network.MacAddress);
    }
    [Fact]
    public void Parse_PausedAndResumedMarkers_AddPausePeriod()
    {
        var parser = new CsvSessionParser();

        var session = parser.Parse(new[]
        {
        "Timestamp,Type,Status,LatencyMs,Target,Detail",
        "2026-07-24T09:00:00Z,MARKER,PAUSED,,,User paused monitoring",
        "2026-07-24T09:05:00Z,MARKER,RESUMED,,,User resumed monitoring"
    });

        var pause = Assert.Single(session.PausePeriods);

        Assert.Equal(
            new DateTime(2026, 7, 24, 9, 0, 0, DateTimeKind.Utc),
            pause.Start);

        Assert.Equal(
            new DateTime(2026, 7, 24, 9, 5, 0, DateTimeKind.Utc),
            pause.End);
    }
}