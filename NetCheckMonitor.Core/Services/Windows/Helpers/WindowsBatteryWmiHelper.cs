using System.Management;
using System.Runtime.Versioning;

namespace NetCheckMonitor.Core.Services.Windows.Helpers;

[SupportedOSPlatform("windows")]
internal static class WindowsBatteryWmiHelper
{
    private static ManagementScope CreateScope()
    {
        ManagementScope scope = new(@"\\.\ROOT\WMI");
        scope.Connect();

        return scope;
    }

    public static ulong GetFullChargeCapacity()
    {
        ManagementScope scope = CreateScope();

        using ManagementObjectSearcher searcher = new(scope, new ObjectQuery("SELECT FullChargedCapacity FROM BatteryFullChargedCapacity"));

        using ManagementObjectCollection results = searcher.Get();

        ManagementObject? battery = results.Cast<ManagementObject>().FirstOrDefault();

        if (battery?["FullChargedCapacity"] is uint capacity)
        {
            return capacity;
        }

        return 0;
    }
    public static ulong GetDesignCapacity()
    {
        ManagementScope scope = CreateScope();

        using ManagementObjectSearcher searcher = new(scope, new ObjectQuery("SELECT DesignedCapacity FROM BatteryStaticData"));

        using ManagementObjectCollection results = searcher.Get();

        ManagementObject? battery = results.Cast<ManagementObject>().FirstOrDefault();

        if (battery?["DesignedCapacity"] is uint capacity)
        {
            return capacity;
        }

        return 0;
    }
    public static int? GetCycleCount()
    {
        ManagementScope scope = CreateScope();

        using ManagementObjectSearcher searcher = new(scope, new ObjectQuery("SELECT CycleCount FROM BatteryCycleCount"));

        using ManagementObjectCollection results = searcher.Get();

        ManagementObject? battery = results.Cast<ManagementObject>().FirstOrDefault();

        if (battery?["CycleCount"] is uint cycleCount)
        {
            return (int)cycleCount;
        }

        return null;
    }
    public static string GetManufacturer()
    {
        ManagementScope scope = CreateScope();

        using ManagementObjectSearcher searcher = new(scope, new ObjectQuery("SELECT ManufactureName FROM BatteryStaticData"));

        using ManagementObjectCollection results = searcher.Get();

        ManagementObject? battery = results.Cast<ManagementObject>().FirstOrDefault();

        return battery?["ManufactureName"]?.ToString() ?? string.Empty;
    }
    public static string GetModel()
    {
        ManagementScope scope = CreateScope();

        using ManagementObjectSearcher searcher = new(scope,new ObjectQuery("SELECT DeviceName FROM BatteryStaticData"));

        using ManagementObjectCollection results = searcher.Get();

        ManagementObject? battery = results.Cast<ManagementObject>().FirstOrDefault();

        return battery?["DeviceName"]?.ToString()?? string.Empty;
    }
}