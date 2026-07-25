namespace NetCheckMonitor.Core.Models.SystemInfo;

public sealed class ProcessorInfo
{
    /// <summary>
    /// CPU 型號
    /// 例如：Apple M3、Intel Core Ultra 7 268V
    /// </summary>
    public string Model { get; init; } = string.Empty;

    /// <summary>
    /// CPU 製造商
    /// 例如：Apple、Intel、AMD
    /// </summary>
    public string Manufacturer { get; init; } = string.Empty;

    /// <summary>
    /// CPU 架構
    /// 例如：ARM64、x64
    /// </summary>
    public string Architecture { get; init; } = string.Empty;

    /// <summary>
    /// 實體核心數
    /// </summary>
    public int? PhysicalCoreCount { get; init; }

    /// <summary>
    /// 邏輯核心數
    /// </summary>
    public int LogicalCoreCount { get; init; }

    /// <summary>
    /// 最大時脈（MHz）
    /// </summary>
    public double? MaxClockSpeedMhz { get; init; }
}