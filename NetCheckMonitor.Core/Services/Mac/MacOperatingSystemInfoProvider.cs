namespace NetCheckMonitor.Core.Services.Mac;

using NetCheckMonitor.Core.Models.SystemInfo;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

[SupportedOSPlatform("macos")]
internal static class MacOperatingSystemInfoProvider
{
    public static OperatingSystemInfo GetOperatingSystemInfo()
    {
        return new OperatingSystemInfo
        {
            Name = MacCommandRunner.Run(
                "/usr/bin/sw_vers",
                "-productName"),

            Version = MacCommandRunner.Run(
                "/usr/bin/sw_vers",
                "-productVersion"),

            Build = MacCommandRunner.Run(
                "/usr/bin/sw_vers",
                "-buildVersion"),

            Architecture = RuntimeInformation.OSArchitecture.ToString(),

            DotNetRuntime = Environment.Version.ToString(),
        };
    }
}