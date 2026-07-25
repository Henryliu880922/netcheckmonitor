namespace NetCheckMonitor.Core.Models.SystemInfo;

public sealed class SystemInfo
{
    /// <summary>
    /// 電腦硬體基本資訊
    /// </summary>
    public ComputerInfo Computer { get; init; } = new();

    /// <summary>
    /// 作業系統資訊
    /// </summary>
    public OperatingSystemInfo OperatingSystem { get; init; } = new();

    /// <summary>
    /// 處理器資訊
    /// </summary>
    public ProcessorInfo Processor { get; init; } = new();

    /// <summary>
    /// 記憶體資訊
    /// </summary>
    public MemoryInfo Memory { get; init; } = new();

    /// <summary>
    /// 網路介面資訊
    /// </summary>
    public IReadOnlyList<NetworkAdapterInfo> NetworkAdapters { get; init; }
        = Array.Empty<NetworkAdapterInfo>();

    /// <summary>
    /// 儲存裝置資訊
    /// </summary>
    public IReadOnlyList<StorageInfo> StorageDevices { get; init; }
        = Array.Empty<StorageInfo>();

    /// <summary>
    /// 電池資訊
    /// 桌上型電腦或無電池裝置可能為 null
    /// </summary>
    public BatteryInfo? Battery { get; init; }

    /// <summary>
    /// 韌體資訊
    /// </summary>
    public FirmwareInfo Firmware { get; init; } = new();

    /// <summary>
    /// 系統安全狀態
    /// </summary>
    public SecurityInfo Security { get; init; } = new();

    /// <summary>
    /// 顯示器資訊
    /// </summary>
    public IReadOnlyList<DisplayInfo> Displays { get; init; }
        = Array.Empty<DisplayInfo>();
}