namespace NetCheckMonitor.Core.Services.Mac;

using System.Runtime.Versioning;
using NetCheckMonitor.Core.Models.SystemInfo;

[SupportedOSPlatform("macos")]
internal static class MacSecurityInfoProvider
{
    public static SecurityInfo GetSecurityInfo()
    {
        if (!OperatingSystem.IsMacOS())
        {
            throw new PlatformNotSupportedException();
        }

        return new SecurityInfo
        {
            IsSecureBootEnabled = true,
            IsTpmPresent = true,
            TpmVersion = "Apple Secure Enclave",
            IsDiskEncryptionEnabled = GetFileVaultStatus(),
            IsFirewallEnabled = GetFirewallStatus(),
            IsSystemIntegrityProtectionEnabled = GetSipStatus()
        };
    }

    private static bool? GetSipStatus()
    {
        try
        {
            string output = MacCommandRunner.Run(
                "/usr/bin/csrutil",
                "status");

            return output.Contains(
                "System Integrity Protection status: enabled",
                StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return null;
        }
    }

    private static bool? GetFileVaultStatus()
    {
        try
        {
            string output = MacCommandRunner.Run(
                "/usr/bin/fdesetup",
                "status");

            return output.StartsWith(
                "Firewall is enabled",
                StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return null;
        }
    }

    private static bool? GetFirewallStatus()
    {
        try
        {
            string output = MacCommandRunner.Run(
                "/usr/libexec/ApplicationFirewall/socketfilterfw",
                "--getglobalstate");

            return output.Contains(
                "enabled",
                StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return null;
        }
    }
}