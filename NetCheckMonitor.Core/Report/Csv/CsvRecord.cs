namespace NetCheckMonitor.Core.Report.Csv;

public sealed class CsvRecord
{
    public DateTime Timestamp { get; init; }

    public string Type { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public long LatencyMs { get; init; }

    public string Target { get; init; } = string.Empty;

    public string Detail { get; init; } = string.Empty;
}