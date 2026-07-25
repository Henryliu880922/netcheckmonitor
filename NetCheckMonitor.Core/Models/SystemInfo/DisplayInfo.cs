namespace NetCheckMonitor.Core.Models.SystemInfo;

public sealed class DisplayInfo
{
    /// <summary>
    /// 顯示器名稱
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// 顯示器製造商
    /// </summary>
    public string Manufacturer { get; init; } = string.Empty;

    /// <summary>
    /// 顯示器型號
    /// </summary>
    public string Model { get; init; } = string.Empty;

    /// <summary>
    /// 顯示器序號
    /// </summary>
    public string SerialNumber { get; init; } = string.Empty;

    /// <summary>
    /// 水平解析度（Pixels）
    /// </summary>
    public int Width { get; init; }

    /// <summary>
    /// 垂直解析度（Pixels）
    /// </summary>
    public int Height { get; init; }

    /// <summary>
    /// 更新率（Hz）
    /// 若無法取得則為 null
    /// </summary>
    public double? RefreshRate { get; init; }

    /// <summary>
    /// 是否為主要顯示器
    /// </summary>
    public bool IsPrimary { get; init; }
}