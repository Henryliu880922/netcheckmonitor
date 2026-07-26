namespace NetCheckMonitor.Core.Services.Mac;

using NetCheckMonitor.Core.Models.SystemInfo;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

[SupportedOSPlatform("macos")]
internal static class MacProcessorInfoProvider
{
    public static ProcessorInfo GetProcessorInfo()
    {
        return new ProcessorInfo
        {
            Model = MacCommandRunner.Run(
                "/usr/sbin/sysctl",
                "-n",
                "machdep.cpu.brand_string"),

            Manufacturer = "Apple",

            Architecture = RuntimeInformation.ProcessArchitecture.ToString(),

            PhysicalCoreCount = GetIntValue(
                "hw.physicalcpu"),

            LogicalCoreCount = GetIntValue(
                "hw.logicalcpu"),

            MaxClockSpeedMhz = 0,
        };
    }

    private static int GetIntValue(string key)
    {
        var value = MacCommandRunner.Run(
            "/usr/sbin/sysctl",
            "-n",
            key);

        return int.TryParse(
            value,
            out var result)
            ? result
            : 0;
    }

}