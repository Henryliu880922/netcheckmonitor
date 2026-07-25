using System.Management;
using NetCheckMonitor.Core.Models.SystemInfo;

namespace NetCheckMonitor.Core.Services.Windows;

public sealed class WindowsBatteryInfoProvider
{
    public Task<BatteryInfo> GetBatteryInfoAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new BatteryInfo());
    }
}