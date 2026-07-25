namespace NetCheckMonitor.Core.Models.SystemInfo;

public sealed class BatteryInfo
{
    /// <summary>
    /// 是否有安裝電池
    /// </summary>
    public bool IsPresent { get; init; }

    /// <summary>
    /// 是否正在充電
    /// </summary>
    public bool IsCharging { get; init; }

    /// <summary>
    /// 電池電量（0~100）
    /// </summary>
    public double ChargePercentage { get; init; }

    /// <summary>
    /// 設計容量（mWh）
    /// </summary>
    public ulong DesignCapacity { get; init; }

    /// <summary>
    /// 完整充電容量（mWh）
    /// </summary>
    public ulong FullChargeCapacity { get; init; }

    /// <summary>
    /// 目前容量（mWh）
    /// </summary>
    public ulong CurrentCapacity { get; init; }

    /// <summary>
    /// 電池健康度（0~100）
    /// 若無法取得則為 null
    /// </summary>
    public double? HealthPercentage { get; init; }

    /// <summary>
    /// 電池循環次數
    /// 若無法取得則為 null
    /// </summary>
    public int? CycleCount { get; init; }

    /// <summary>
    /// 電池製造商
    /// </summary>
    public string Manufacturer { get; init; } = string.Empty;

    /// <summary>
    /// 電池型號
    /// </summary>
    public string Model { get; init; } = string.Empty;

    /// <summary>
    /// 預估剩餘使用時間
    /// 若無法取得則為 null
    /// </summary>
    public TimeSpan? RemainingTime { get; init; }
}