namespace NetCheckMonitor.Core.Models.SystemInfo;

public sealed class ComputerInfo
{
    /// <summary>
    /// 電腦名稱（Hostname）
    /// </summary>
    public string HostName { get; init; } = string.Empty;

    /// <summary>
    /// 製造商（Dell、Apple、ASUS...）
    /// </summary>
    public string Manufacturer { get; init; } = string.Empty;

    /// <summary>
    /// 機型（MacBook Pro、Latitude 3410...）
    /// </summary>
    public string Model { get; init; } = string.Empty;

    /// <summary>
    /// 序號
    /// </summary>
    public string SerialNumber { get; init; } = string.Empty;
}