namespace NetCheckMonitor.Core.Services.Windows;

using System.Management;
using System.Runtime.Versioning;
using NetCheckMonitor.Core.Models.SystemInfo;

[SupportedOSPlatform("windows")]
internal static class WindowsStorageInfoProvider
{
    public static IReadOnlyList<StorageInfo> GetStorageInfo()
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException();
        }

        List<StorageInfo> storageDevices = [];

        using ManagementObjectSearcher searcher =
            new("SELECT DeviceID, Manufacturer, Model, SerialNumber, " + "FirmwareRevision, Size, MediaType, InterfaceType, " + "Capabilities " + "FROM Win32_DiskDrive");

        foreach (ManagementObject disk in searcher.Get())
        {
            string deviceId = disk["DeviceID"]?.ToString() ?? string.Empty;

            (ulong availableBytes, ulong usedBytes) = GetDiskUsage(deviceId);
            bool isRemovable = false;

            bool isSystemDisk = IsSystemDisk(deviceId);

            if (disk["Capabilities"] is ushort[] capabilities)
            {
                isRemovable = capabilities.Contains((ushort)7);
            }

            storageDevices.Add(
                new StorageInfo
                {
                    DeviceName = deviceId,
                    Manufacturer = disk["Manufacturer"]?.ToString() ?? string.Empty,
                    Model = disk["Model"]?.ToString() ?? string.Empty,
                    SerialNumber = disk["SerialNumber"]?.ToString()?.Trim() ?? string.Empty,
                    BusType = disk["InterfaceType"]?.ToString() ?? string.Empty,
                    FirmwareVersion = disk["FirmwareRevision"]?.ToString() ?? string.Empty,
                    MediaType = disk["MediaType"]?.ToString() ?? string.Empty,
                    TotalBytes = Convert.ToUInt64(disk["Size"] ?? 0),
                    AvailableBytes = availableBytes,
                    UsedBytes = usedBytes,
                    IsSystemDisk = isSystemDisk,
                    IsRemovable = isRemovable,
                });
        }

        return storageDevices;
    }
    private static (ulong AvailableBytes, ulong UsedBytes) GetDiskUsage(string deviceId)
    {
        ulong availableBytes = 0;
        ulong logicalTotalBytes = 0;

        string escapedDeviceId = deviceId.Replace("\\", "\\\\");

        using ManagementObjectSearcher partitionSearcher =
            new("ASSOCIATORS OF " + $"{{Win32_DiskDrive.DeviceID='{escapedDeviceId}'}} " + "WHERE AssocClass=Win32_DiskDriveToDiskPartition");

        foreach (ManagementObject partition in partitionSearcher.Get())
        {
            string partitionDeviceId = partition["DeviceID"]?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(partitionDeviceId))
            {
                continue;
            }

            string escapedPartitionDeviceId =
                partitionDeviceId.Replace("\\", "\\\\");

            using ManagementObjectSearcher logicalDiskSearcher =
                new("ASSOCIATORS OF " + $"{{Win32_DiskPartition.DeviceID='{escapedPartitionDeviceId}'}} " + "WHERE AssocClass=Win32_LogicalDiskToPartition");

            foreach (ManagementObject logicalDisk in logicalDiskSearcher.Get())
            {
                logicalTotalBytes += Convert.ToUInt64(logicalDisk["Size"] ?? 0);
                availableBytes += Convert.ToUInt64(logicalDisk["FreeSpace"] ?? 0);
            }
        }

        ulong usedBytes = logicalTotalBytes >= availableBytes ? logicalTotalBytes - availableBytes : 0;

        return (availableBytes, usedBytes);
    }
    private static bool IsSystemDisk(string deviceId)
    {
        string systemDrive = Environment.GetEnvironmentVariable("SystemDrive") ?? "C:";

        string escapedDeviceId =
            deviceId.Replace("\\", "\\\\");

        using ManagementObjectSearcher partitionSearcher =
            new("ASSOCIATORS OF " + $"{{Win32_DiskDrive.DeviceID='{escapedDeviceId}'}} " + "WHERE AssocClass=Win32_DiskDriveToDiskPartition");

        foreach (ManagementObject partition in partitionSearcher.Get())
        {
            string partitionDeviceId =
                partition["DeviceID"]?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(partitionDeviceId))
            {
                continue;
            }

            string escapedPartitionDeviceId =
                partitionDeviceId.Replace("\\", "\\\\");

            using ManagementObjectSearcher logicalDiskSearcher =
                new("ASSOCIATORS OF " + $"{{Win32_DiskPartition.DeviceID='{escapedPartitionDeviceId}'}} " + "WHERE AssocClass=Win32_LogicalDiskToPartition");

            foreach (ManagementObject logicalDisk in logicalDiskSearcher.Get())
            {
                string logicalDeviceId =
                    logicalDisk["DeviceID"]?.ToString()
                    ?? string.Empty;

                if (string.Equals(
                        logicalDeviceId,
                        systemDrive,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }
        return false;
    }
}