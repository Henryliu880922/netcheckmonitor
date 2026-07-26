namespace NetCheckMonitor.Core.Services.Windows;

using System.Runtime.Versioning;
using NetCheckMonitor.Core.Models.SystemInfo;
using NetCheckMonitor.Core.Services.Windows.Helpers;

[SupportedOSPlatform("windows")]
internal static class WindowsSecurityInfoProvider
{
    public static SecurityInfo GetSecurityInfo()
    {
        return new SecurityInfo
        {
            IsFirewallEnabled = WindowsSecurityHelper.IsFirewallEnabled(),
            IsSecureBootEnabled = WindowsSecurityHelper.IsSecureBootEnabled(),
            IsDiskEncryptionEnabled = WindowsSecurityHelper.IsDiskEncryptionEnabled(),
            IsTpmPresent = WindowsSecurityHelper.IsTpmPresent(),
            TpmVersion = WindowsSecurityHelper.GetTpmVersion(),
            IsSystemIntegrityProtectionEnabled = null
        };
    }
}