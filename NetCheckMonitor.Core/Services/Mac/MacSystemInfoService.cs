namespace NetCheckMonitor.Core.Services.Mac;

using System.Runtime.Versioning;
using NetCheckMonitor.Core.Models.SystemInfo;
using NetCheckMonitor.Core.Services.Interfaces;

[SupportedOSPlatform("macos")]
public sealed class MacSystemInfoService : ISystemInfoService
{
    public Task<SystemInfo> GetSystemInfoAsync(
        CancellationToken cancellationToken = default)
    {
        SystemInfo systemInfo = new()
        {
            Computer = MacComputerInfoProvider.GetComputerInfo(),
            OperatingSystem = MacOperatingSystemInfoProvider.GetOperatingSystemInfo(),
            Processor = MacProcessorInfoProvider.GetProcessorInfo(),
            Memory = MacMemoryInfoProvider.GetMemoryInfo(),
            StorageDevices = MacStorageInfoProvider.GetStorageInfo(),
            NetworkAdapters = new MacNetworkInfoProvider().GetNetworkAdapters(),
            Battery = new MacBatteryInfoProvider().GetBatteryInfo(),
            Firmware = MacFirmwareInfoProvider.GetFirmwareInfo(),
            Security = MacSecurityInfoProvider.GetSecurityInfo(),
            Displays = MacDisplayInfoProvider.GetDisplays(),
        };

        return Task.FromResult(systemInfo);
    }
}