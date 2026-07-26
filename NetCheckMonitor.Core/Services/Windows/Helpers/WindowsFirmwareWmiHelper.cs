using System.Management;
using System.Runtime.Versioning;

namespace NetCheckMonitor.Core.Services.Windows.Helpers;


[SupportedOSPlatform("windows")]

internal static class WindowsFirmwareWmiHelper
{
    public static (string Version, string Manufacturer, DateTime? ReleaseDate) GetBiosInfo()
    {
        if (!OperatingSystem.IsWindows())
        {
            return (string.Empty, string.Empty, null);
        }

        
        using var searcher = new ManagementObjectSearcher("SELECT SMBIOSBIOSVersion, Manufacturer, ReleaseDate FROM Win32_BIOS");

        using var results = searcher.Get();

        foreach (ManagementObject bios in results)
        {
            string version = bios["SMBIOSBIOSVersion"]?.ToString() ?? string.Empty;
            string manufacturer = bios["Manufacturer"]?.ToString() ?? string.Empty;

            DateTime? releaseDate = null;
            string? releaseDateValue = bios["ReleaseDate"]?.ToString();
            if (!string.IsNullOrWhiteSpace(releaseDateValue))
            {
                try
                {
                    releaseDate = ManagementDateTimeConverter.ToDateTime(releaseDateValue);
                }
                catch (ArgumentOutOfRangeException)
                {
                    releaseDate = null;
                }
            }
            return (string.Empty, string.Empty, null);
        }

        return (string.Empty, string.Empty, null);
    }
    
}