public sealed class StorageInfo
{
    /// <summary>
    /// 系統裝置名稱
    /// 例如：PhysicalDrive0、disk0、nvme0n1
    /// </summary>
    public string DeviceName { get; init; } = string.Empty;

    /// <summary>
    /// 磁碟製造商
    /// </summary>
    public string Manufacturer { get; init; } = string.Empty;

    /// <summary>
    /// 磁碟型號
    /// </summary>
    public string Model { get; init; } = string.Empty;

    /// <summary>
    /// 磁碟序號
    /// </summary>
    public string SerialNumber { get; init; } = string.Empty;

    /// <summary>
    /// 匯流排類型
    /// 例如：SATA、NVMe、USB
    /// </summary>
    public string BusType { get; init; } = string.Empty;

    /// <summary>
    /// 儲存媒體類型
    /// 例如：SSD、HDD、Flash
    /// </summary>
    public string MediaType { get; init; } = string.Empty;

    /// <summary>
    /// 韌體版本
    /// </summary>
    public string FirmwareVersion { get; init; } = string.Empty;

    /// <summary>
    /// 總容量（Bytes）
    /// </summary>
    public ulong TotalBytes { get; init; }

    /// <summary>
    /// 可用容量（Bytes）
    /// </summary>
    public ulong AvailableBytes { get; init; }

    /// <summary>
    /// 已使用容量（Bytes）
    /// </summary>
    public ulong UsedBytes { get; init; }

    /// <summary>
    /// 磁碟健康度（0~100）
    /// 若無法取得則為 null
    /// </summary>
    public double? HealthPercentage { get; init; }

    /// <summary>
    /// 是否為系統磁碟
    /// </summary>
    public bool IsSystemDisk { get; init; }

    /// <summary>
    /// 是否為可移除式裝置
    /// </summary>
    public bool IsRemovable { get; init; }
}