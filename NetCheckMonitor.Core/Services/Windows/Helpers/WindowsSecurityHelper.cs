namespace NetCheckMonitor.Core.Services.Windows.Helpers;

using System.Runtime.Versioning;
using Microsoft.Win32;
using System.Management;

[SupportedOSPlatform("windows")]
internal static class WindowsSecurityHelper
{
    public static bool? IsFirewallEnabled()

    {
        using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\SharedAccess\Parameters\FirewallPolicy\StandardProfile");

        if (key == null)
        {
            return null;
        }
        return (int?)key.GetValue("EnableFirewall") == 1;
    }
    public static bool? IsSecureBootEnabled()
    {
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\SecureBoot\State");

            if (key == null)
            {
                return null;
            }
            return (int?)key.GetValue("UEFISecureBootEnabled") == 1;
        }
        catch
        {
            return null;
        }
    }
    public static bool? IsDiskEncryptionEnabled()
    {
        try
        {
            var systemDrive = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System));

            if (string.IsNullOrWhiteSpace(systemDrive))
            {
                return null;
            }

            var driveLetter = systemDrive.TrimEnd('\\');

            var scope = new ManagementScope(@"\\.\ROOT\CIMV2\Security\MicrosoftVolumeEncryption");

            scope.Connect();

            using var searcher = new ManagementObjectSearcher(
                scope,
                new ObjectQuery("SELECT DriveLetter, ProtectionStatus " + "FROM Win32_EncryptableVolume"));

            using var results = searcher.Get();

            foreach (ManagementObject volume in results)
            {
                var volumeDriveLetter = volume["DriveLetter"]?.ToString();

                if (!string.Equals(volumeDriveLetter, driveLetter, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var protectionStatus = Convert.ToUInt32(volume["ProtectionStatus"]);

                return protectionStatus == 1;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
    public static bool? IsTpmPresent()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher(@"ROOT\CIMV2\Security\MicrosoftTpm", "SELECT IsEnabled_InitialValue FROM Win32_Tpm");

            using var results = searcher.Get();

            foreach (ManagementObject tpm in results)
            {
                return (bool?)tpm["IsEnabled_InitialValue"];
            }
            return false;
        }
        catch
        {
            return null;
        }
    }
    public static string GetTpmVersion()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher(@"ROOT\CIMV2\Security\MicrosoftTpm","SELECT SpecVersion FROM Win32_Tpm");

            using var results = searcher.Get();

            foreach (ManagementObject tpm in results)

            {
                return tpm["SpecVersion"]?.ToString() ?? string.Empty;
            }
            return string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}