namespace NetCheckMonitor.Core.Services.Windows;

using System.Management;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using NetCheckMonitor.Core.Models.SystemInfo;

[SupportedOSPlatform("windows")]
internal static class WindowsProcessorInfoProvider
{
    public static ProcessorInfo GetProcessorInfo()
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException();
        }

        using ManagementObjectSearcher searcher =
            new("SELECT Name, Manufacturer, NumberOfCores, " + "NumberOfLogicalProcessors, MaxClockSpeed " + "FROM Win32_Processor");

        foreach (ManagementObject processor in searcher.Get())
        {
            return new ProcessorInfo
            {
                Model = processor["Name"]?.ToString() ?? string.Empty,
                Manufacturer = processor["Manufacturer"]?.ToString() ?? string.Empty,
                Architecture = RuntimeInformation.ProcessArchitecture.ToString(),
                PhysicalCoreCount = Convert.ToInt32(processor["NumberOfCores"] ?? 0),
                LogicalCoreCount = Convert.ToInt32(processor["NumberOfLogicalProcessors"] ?? 0),
                MaxClockSpeedMhz = Convert.ToDouble(processor["MaxClockSpeed"] ?? 0),
            };
        }
        return new ProcessorInfo
        {
            Architecture = RuntimeInformation.ProcessArchitecture.ToString(),
        };
    }
}