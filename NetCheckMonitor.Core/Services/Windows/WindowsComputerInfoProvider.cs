namespace NetCheckMonitor.Core.Services.Windows;

using Microsoft.Win32;
using NetCheckMonitor.Core.Models.SystemInfo;

internal static class WindowsComputerInfoProvider
{
    public static ComputerInfo GetComputerInfo()
    {
        return new ComputerInfo
        {
            HostName = Environment.MachineName,
            Manufacturer = ReadRegistryValue(
                @"HARDWARE\DESCRIPTION\System\BIOS",
                "SystemManufacturer"),
            Model = ReadRegistryValue(
                @"HARDWARE\DESCRIPTION\System\BIOS",
                "SystemProductName"),
            SerialNumber = ReadRegistryValue(
                @"HARDWARE\DESCRIPTION\System\BIOS",
                "SystemSerialNumber"),
        };
    }

    private static string ReadRegistryValue(
        string subKeyPath,
        string valueName)
    {
        if (!OperatingSystem.IsWindows())
        {
            return string.Empty;
        }

        using var key = Registry.LocalMachine.OpenSubKey(subKeyPath);

        return key?.GetValue(valueName)?.ToString() ?? string.Empty;
    }
}