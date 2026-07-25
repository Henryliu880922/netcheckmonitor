namespace NetCheckMonitor.Core.Services.Windows;

using NetCheckMonitor.Core.Models.SystemInfo;
using NetCheckMonitor.Core.Services.Interfaces;

public sealed class WindowsSystemInfoService : ISystemInfoService
{
    /// <inheritdoc/>
    public Task<SystemInfo> GetSystemInfoAsync(
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}