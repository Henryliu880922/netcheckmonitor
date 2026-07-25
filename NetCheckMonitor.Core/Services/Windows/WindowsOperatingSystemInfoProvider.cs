namespace NetCheckMonitor.Core.Services.Windows;

using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Win32;
using NetCheckMonitor.Core.Models.SystemInfo;
[SupportedOSPlatform("windows")]

internal static class WindowsOperatingSystemInfoProvider
{
    public static OperatingSystemInfo GetOperatingSystemInfo()
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException();
        }
        using RegistryKey? currentVersionKey =
            Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
        
        DateTime now = DateTime.Now;
        DateTime bootTime =
            DateTime.Now - TimeSpan.FromMilliseconds(Environment.TickCount64);
        
        return new OperatingSystemInfo
        {
            Name = currentVersionKey?.GetValue("ProductName")?.ToString()
                ?? "Unknown Windows",
            Version = currentVersionKey?.GetValue("DisplayVersion")?.ToString()
                ?? "Unknown",
            Build = currentVersionKey?.GetValue("CurrentBuild")?.ToString()
                ?? "Unknown",
            Architecture = RuntimeInformation.OSArchitecture.ToString(),
            DotNetRuntime = RuntimeInformation.FrameworkDescription,
            Uptime = now - bootTime,
        };
    }
}