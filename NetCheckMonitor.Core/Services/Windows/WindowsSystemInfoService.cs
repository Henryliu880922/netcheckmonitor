namespace NetCheckMonitor.Core.Services.Windows;

using NetCheckMonitor.Core.Models.SystemInfo;
using NetCheckMonitor.Core.Services.Interfaces;
using System.Runtime.Versioning;

[SupportedOSPlatform("windows")]

public sealed class WindowsSystemInfoService : ISystemInfoService
{
    /// <inheritdoc/>
    public Task<SystemInfo> GetSystemInfoAsync(
        CancellationToken cancellationToken = default)
    {
        var systemInfo = new SystemInfo
        {
            Computer = WindowsComputerInfoProvider.GetComputerInfo(),
            OperatingSystem = WindowsOperatingSystemInfoProvider.GetOperatingSystemInfo(),
            Processor = WindowsProcessorInfoProvider.GetProcessorInfo(),
            Memory = WindowsMemoryInfoProvider.GetMemoryInfo(),
            StorageDevices = WindowsStorageInfoProvider.GetStorageInfo(),
            Displays = WindowsDisplayInfoProvider.GetDisplays(),
            NetworkAdapters = new WindowsNetworkInfoProvider().GetNetworkAdapters(),
            Firmware = WindowsFirmwareInfoProvider.GetFirmwareInfo(),
            Security = WindowsSecurityInfoProvider.GetSecurityInfo(),
            Battery = new WindowsBatteryInfoProvider().GetBatteryInfo(),
        };

        return Task.FromResult(systemInfo);
    }
}