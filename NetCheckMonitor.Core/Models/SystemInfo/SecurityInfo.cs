namespace NetCheckMonitor.Core.Models.SystemInfo;

public sealed class SecurityInfo
{
    /// <summary>
    /// 是否啟用 Secure Boot
    /// 若無法判斷則為 null
    /// </summary>
    public bool? IsSecureBootEnabled { get; init; }

    /// <summary>
    /// 是否存在 TPM
    /// 若無法判斷則為 null
    /// </summary>
    public bool? IsTpmPresent { get; init; }

    /// <summary>
    /// TPM 版本
    /// 例如：1.2、2.0
    /// </summary>
    public string TpmVersion { get; init; } = string.Empty;

    /// <summary>
    /// 是否啟用磁碟加密
    /// 若無法判斷則為 null
    /// </summary>
    public bool? IsDiskEncryptionEnabled { get; init; }

    /// <summary>
    /// 是否啟用防火牆
    /// 若無法判斷則為 null
    /// </summary>
    public bool? IsFirewallEnabled { get; init; }

    /// <summary>
    /// 是否啟用系統完整性保護
    /// 若無法判斷則為 null
    /// </summary>
    public bool? IsSystemIntegrityProtectionEnabled { get; init; }
}