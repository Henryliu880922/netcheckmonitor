namespace NetCheckMonitor.Core.Services.Mac;

using NetCheckMonitor.Core.Models.SystemInfo;
using System.Runtime.Versioning;

[SupportedOSPlatform("macos")]
internal static class MacComputerInfoProvider
{
    public static ComputerInfo GetComputerInfo()
    {
        return new ComputerInfo
        {
            HostName = Environment.MachineName,
            Manufacturer = "Apple Inc.",
            Model = MacCommandRunner.Run(
                "/usr/sbin/sysctl",
                "-n",
                "hw.model"),
            SerialNumber = GetSerialNumber(),
        };
    }

    private static string GetSerialNumber()
    {
        var output = MacCommandRunner.Run(
            "/usr/sbin/system_profiler",
            "SPHardwareDataType");

        foreach (var line in output.Split('\n'))
        {
            var trimmed = line.Trim();

            if (!trimmed.StartsWith(
                    "Serial Number",
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var separatorIndex = trimmed.IndexOf(':');

            if (separatorIndex < 0)
            {
                continue;
            }

            return trimmed[(separatorIndex + 1)..].Trim();
        }

        return string.Empty;
    }
}