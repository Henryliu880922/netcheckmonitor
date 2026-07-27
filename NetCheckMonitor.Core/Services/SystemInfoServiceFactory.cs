namespace NetCheckMonitor.Core.Services;

using NetCheckMonitor.Core.Services.Interfaces;
using NetCheckMonitor.Core.Services.Mac;
using NetCheckMonitor.Core.Services.Windows;

public static class SystemInfoServiceFactory
{
    public static ISystemInfoService Create()
    {
        if (OperatingSystem.IsWindows())
        {
            return new WindowsSystemInfoService();
        }

        if (OperatingSystem.IsMacOS())
        {
            return new MacSystemInfoService();
        }

        throw new PlatformNotSupportedException(
            "NetCheckMonitor currently supports Windows and macOS only.");
    }
}
