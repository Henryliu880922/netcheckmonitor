using NetCheckMonitor.Core.Models.SystemInfo;
using NetCheckMonitor.Core.Services.Windows.Helpers;
using System.Runtime.Versioning;

namespace NetCheckMonitor.Core.Services.Windows;

[SupportedOSPlatform("windows")]

internal static class WindowsFirmwareInfoProvider
{
    public static FirmwareInfo GetFirmwareInfo()
    {
        var biosInfo = WindowsFirmwareWmiHelper.GetBiosInfo();
        return new FirmwareInfo
        {
            Version = biosInfo.Version,
            Manufacturer = biosInfo.Manufacturer,
            ReleaseDate = biosInfo.ReleaseDate,
        };
    }
    
}