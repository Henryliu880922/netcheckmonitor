namespace NetCheckMonitor.Core.Models.SystemInfo;

public sealed class OperatingSystemInfo
{
    /// <summary>
    /// 作業系統名稱
    /// 例如：Windows、macOS、Ubuntu
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// 作業系統版本
    /// 例如：11、15.6、24.04
    /// </summary>
    public string Version { get; init; } = string.Empty;

    /// <summary>
    /// Build Number
    /// 例如：26100、24G84
    /// </summary>
    public string Build { get; init; } = string.Empty;

    /// <summary>
    /// 系統架構
    /// 例如：ARM64、x64
    /// </summary>
    public string Architecture { get; init; } = string.Empty;

    /// <summary>
    /// .NET Runtime
    /// 例如：.NET 10.0.0
    /// </summary>
    public string DotNetRuntime { get; init; } = string.Empty;

    /// <summary>
    /// 開機時間
    /// </summary>
    public DateTime BootTime { get; init; }

    /// <summary>
    /// 系統運作時間
    /// </summary>
    public TimeSpan Uptime { get; init; }
}