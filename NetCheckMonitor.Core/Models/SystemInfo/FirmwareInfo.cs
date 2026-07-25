namespace NetCheckMonitor.Core.Models.SystemInfo;

public sealed class FirmwareInfo
{
    /// <summary>
    /// 韌體類型
    /// 例如：BIOS、UEFI
    /// </summary>
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// 韌體版本
    /// </summary>
    public string Version { get; init; } = string.Empty;

    /// <summary>
    /// 韌體製造商
    /// </summary>
    public string Manufacturer { get; init; } = string.Empty;

    /// <summary>
    /// 韌體發布日期
    /// 若無法取得則為 null
    /// </summary>
    public DateTime? ReleaseDate { get; init; }

    /// <summary>
    /// 是否使用 UEFI 模式啟動
    /// 若無法判斷則為 null
    /// </summary>
    public bool? IsUefi { get; init; }
}