namespace NetCheckMonitor.Core;

public sealed class EventNote
{
    public DateTime Time { get; set; }

    public string Text { get; set; } = string.Empty;
}

public sealed class MonitoringRecord
{
    public DateTime Time { get; set; }

    public bool Online { get; set; }

    public string Status { get; set; } = string.Empty;

    public long Latency { get; set; }

    public string Target { get; set; } = string.Empty;

    public string Detail { get; set; } = string.Empty;
}

public sealed class MonitoringPeriod
{
    public DateTime Start { get; set; }

    public DateTime End { get; set; }
}

public sealed class OutageRecord
{
    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public int Count { get; set; }

    public string Machine { get; set; } = string.Empty;

    public TimeSpan Duration { get; set; }
}

public sealed class MonitoringSession
{
    public string MachineName { get; set; } = Environment.MachineName;

    public string MachineId { get; set; } = "LEGACY";

    public DateTime Start { get; set; } = DateTime.MaxValue;

    public DateTime End { get; set; } = DateTime.MinValue;

    public bool Stopped { get; set; }

    public string AdapterName { get; set; } = string.Empty;

    public string AdapterDescription { get; set; } = string.Empty;

    public string ConnectionType { get; set; } = "Disconnected";

    public int WifiSignal { get; set; } = -1;

    public List<MonitoringRecord> Records { get; set; } = new();

    public List<MonitoringPeriod> Pauses { get; set; } = new();

    public List<EventNote> EventNotes { get; set; } = new();

    public List<NetworkInfo> Networks { get; } = [];

    public string SourceFile { get; set; } = string.Empty;
}

public sealed class NetworkInfo
{
    public string Adapter { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Ssid { get; set; } = string.Empty;

    public string Bssid { get; set; } = string.Empty;

    public int Signal { get; set; }
}

public sealed class DailyReportData
{
    public string MachineName { get; set; } = string.Empty;

    public string MachineId { get; set; } = string.Empty;

    public DateTime Day { get; set; }

    public TimeSpan Effective { get; set; }

    public TimeSpan Outage { get; set; }

    public int Checks { get; set; }

    public int Offline { get; set; }

    public int OutageEvents { get; set; }

    public TimeSpan LongestOutage { get; set; }

    public List<MonitoringRecord> Records { get; set; } = new();

    public List<MonitoringPeriod> Pauses { get; set; } = new();

    public List<EventNote> EventNotes { get; set; } = new();
}