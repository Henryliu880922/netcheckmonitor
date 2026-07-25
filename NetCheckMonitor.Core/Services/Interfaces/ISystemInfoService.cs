namespace NetCheckMonitor.Core.Services.Interfaces;

using NetCheckMonitor.Core.Models.SystemInfo;

public interface ISystemInfoService
{
    /// <summary>
    /// 取得系統資訊。
    /// </summary>
    /// <param name="cancellationToken">取消權杖。</param>
    /// <returns>系統資訊。</returns>
    Task<SystemInfo> GetSystemInfoAsync(
        CancellationToken cancellationToken = default);
}