namespace NetCheckMonitor.Core.Services.Windows;

using System.Management;
using System.Runtime.Versioning;
using NetCheckMonitor.Core.Models.SystemInfo;

[SupportedOSPlatform("windows")]
internal static class WindowsMemoryInfoProvider
{
    public static MemoryInfo GetMemoryInfo()
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException();
        }

        using ManagementObjectSearcher operatingSystemSearcher =
            new("SELECT TotalVisibleMemorySize, FreePhysicalMemory " +"FROM Win32_OperatingSystem");
        using ManagementObjectSearcher physicalMemorySearcher =
            new("SELECT SMBIOSMemoryType " +"FROM Win32_PhysicalMemory");

        int moduleCount = 0;
        string memoryType = "Unknown";

        foreach (ManagementObject memory in physicalMemorySearcher.Get())
        {
            moduleCount++;
            ushort type = Convert.ToUInt16(memory["SMBIOSMemoryType"] ?? 0);
            
            string currentType = type switch
            {
                20 => "DDR",
                21 => "DDR2",
                24 => "DDR3",
                26 => "DDR4",
                34 => "DDR5",
                _ => "Unknown",
            };

            if (memoryType == "Unknown" &&
                currentType != "Unknown")
            {
                memoryType = currentType;
            }
        }

        foreach (
            ManagementObject operatingSystem
            in operatingSystemSearcher.Get())
        {
            ulong totalBytes = Convert.ToUInt64(operatingSystem["TotalVisibleMemorySize"] ?? 0)* 1024;

            ulong availableBytes = Convert.ToUInt64(operatingSystem["FreePhysicalMemory"] ?? 0)* 1024;

            ulong usedBytes = totalBytes >= availableBytes? totalBytes - availableBytes: 0;

            double usagePercentage = totalBytes > 0? (double)usedBytes / totalBytes * 100: 0;

            return new MemoryInfo
            {
                TotalBytes = totalBytes,
                AvailableBytes = availableBytes,
                UsedBytes = usedBytes,
                UsagePercentage = usagePercentage,
                MemoryType = memoryType,
                ModuleCount = moduleCount,
            };
        }

        return new MemoryInfo
        {
            MemoryType = memoryType,
            ModuleCount = moduleCount,
        };
    }
}