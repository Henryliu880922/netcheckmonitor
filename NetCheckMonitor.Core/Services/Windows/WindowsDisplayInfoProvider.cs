using System.Runtime.Versioning;
using System.Runtime.InteropServices;
using NetCheckMonitor.Core.Services.Windows.Interop;

namespace NetCheckMonitor.Core.Services.Windows;

using NetCheckMonitor.Core.Models.SystemInfo;

[SupportedOSPlatform("windows")]
internal static class WindowsDisplayInfoProvider
{
    public static IReadOnlyList<DisplayInfo> GetDisplays()
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException();
        }

        NativeMethods.DEVMODE devMode = new();
        devMode.dmSize = (short)Marshal.SizeOf<NativeMethods.DEVMODE>();

        bool success = NativeMethods.EnumDisplaySettings(
            null,
            NativeMethods.ENUM_CURRENT_SETTINGS,
            ref devMode);

        if (!success)
        {
            return [];
        }
        NativeMethods.DISPLAY_DEVICE displayDevice = new();
        displayDevice.cb = Marshal.SizeOf<NativeMethods.DISPLAY_DEVICE>();

        bool deviceSuccess = NativeMethods.EnumDisplayDevices(
            null,
            0,
            ref displayDevice,
            0);

        if (!deviceSuccess)
        {
            return [];
        }
        DisplayInfo display = new()
        {
            Name = displayDevice.DeviceString,
            Width = devMode.dmPelsWidth,
            Height = devMode.dmPelsHeight,
            RefreshRate = devMode.dmDisplayFrequency > 0? devMode.dmDisplayFrequency: null,
            IsPrimary = true
        };
        return [display];
    }
}