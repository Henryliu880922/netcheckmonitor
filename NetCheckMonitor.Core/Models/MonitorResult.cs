namespace NetCheckMonitor.Core.Models;

public class MonitorResult
{
    public DateTime Timestamp { get; init; }

    public string Host { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public long? RoundTripTime { get; init; }
}