namespace NetCheckMonitor.Core.Models.SystemInfo;

public sealed class MemoryInfo
{
    /// <summary>
    /// 實體記憶體總容量（Bytes）
    /// </summary>
    public ulong TotalBytes { get; init; }

    /// <summary>
    /// 可用實體記憶體容量（Bytes）
    /// </summary>
    public ulong AvailableBytes { get; init; }

    /// <summary>
    /// 已使用實體記憶體容量（Bytes）
    /// </summary>
    public ulong UsedBytes { get; init; }

    /// <summary>
    /// 記憶體使用率（0~100）
    /// </summary>
    public double UsagePercentage { get; init; }

    /// <summary>
    /// 記憶體類型
    /// 例如：DDR4、DDR5、LPDDR5X
    /// </summary>
    public string MemoryType { get; init; } = string.Empty;

    /// <summary>
    /// 已安裝的記憶體模組數量
    /// 若無法取得則為 null
    /// </summary>
    public int? ModuleCount { get; init; }
}