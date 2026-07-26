using System.Management;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Versioning;
using NetCheckMonitor.Core.Models.SystemInfo;
using NetCheckMonitor.Core.Services.Windows.Helpers;

namespace NetCheckMonitor.Core.Services.Windows;

[SupportedOSPlatform("windows")]
public sealed class WindowsBatteryInfoProvider
{
    private enum BatteryStatus : ushort
    {
        /// <summary>放電中。</summary>
        Discharging = 1,
        /// <summary>已接上 AC 電源，但未充電。</summary>
        AcConnected = 2,
        /// <summary>已充滿。</summary>
        FullyCharged = 3,
        /// <summary>低電量。</summary>
        Low = 4,
        /// <summary>電量嚴重不足。</summary>
        Critical = 5,
        /// <summary>充電中。</summary>
        Charging = 6,
        /// <summary>充電中（高電量）。</summary>
        ChargingAndHigh = 7,
        /// <summary>充電中（低電量）。</summary>
        ChargingAndLow = 8,
        /// <summary>充電中（電量嚴重不足）。</summary>
        ChargingAndCritical = 9,
        /// <summary>狀態未知。</summary>
        Undefined = 10,
        /// <summary>部分充電。</summary>
        PartiallyCharged = 11
    }
    public BatteryInfo GetBatteryInfo()
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException();
        }

        using ManagementObjectSearcher searcher = new("SELECT * FROM Win32_Battery");
        using ManagementObjectCollection batteries = searcher.Get();
        ManagementObject? battery = batteries.Cast<ManagementObject>().FirstOrDefault();
        bool isPresent = battery is not null;

        bool isCharging = false;

        double chargePercentage = 0;

        if (battery is not null)
        {
            object? estimatedChargeRemaining = battery["EstimatedChargeRemaining"];
            if (estimatedChargeRemaining is ushort percentage)
            {
                chargePercentage = percentage;
            }
        }

        if (battery is not null)
        {
            object? value = battery["BatteryStatus"];

            if (value is ushort statusValue)
            {
                BatteryStatus status = (BatteryStatus)statusValue;
                isCharging = status is
                   BatteryStatus.Charging
                    or BatteryStatus.ChargingAndHigh
                    or BatteryStatus.ChargingAndLow
                    or BatteryStatus.ChargingAndCritical;
            }
        }

        ulong designCapacity = WindowsBatteryWmiHelper.GetDesignCapacity();
        ulong fullChargeCapacity = WindowsBatteryWmiHelper.GetFullChargeCapacity();
        int? cycleCount = WindowsBatteryWmiHelper.GetCycleCount();
        string manufacturer = WindowsBatteryWmiHelper.GetManufacturer();
        string model = WindowsBatteryWmiHelper.GetModel();
        ulong currentCapacity = (ulong)Math.Round(fullChargeCapacity * chargePercentage / 100.0);
        
        double? healthPercentage = null;
        if (designCapacity > 0)
        {
            healthPercentage =
                fullChargeCapacity * 100.0 / designCapacity;
        }

        return new BatteryInfo
        {
            IsPresent = isPresent,
            IsCharging = isCharging,
            ChargePercentage = chargePercentage,
            DesignCapacity = designCapacity,
            FullChargeCapacity = fullChargeCapacity,
            CycleCount = cycleCount,
            CurrentCapacity = currentCapacity,
            HealthPercentage = healthPercentage,
            Manufacturer = manufacturer,
            Model = model
        };
    }
}