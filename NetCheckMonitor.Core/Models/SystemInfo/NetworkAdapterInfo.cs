namespace NetCheckMonitor.Core.Models.SystemInfo;

public sealed class NetworkAdapterInfo
{
    /// <summary>
    /// 網路介面索引
    /// </summary>
    public int Index { get; init; }

    /// <summary>
    /// 網路介面唯一識別碼
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// 網路介面名稱
    /// 例如：Wi-Fi、Ethernet、en0
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// 網路介面描述
    /// 例如：Intel(R) Wi-Fi 7 BE202
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// 網路介面類型
    /// Ethernet、Wi-Fi、VPN...
    /// </summary>
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// 是否已啟用
    /// </summary>
    public bool IsUp { get; init; }

    /// <summary>
    /// 是否為預設網路介面
    /// </summary>
    public bool IsDefault { get; init; }

    /// <summary>
    /// 是否為虛擬網路介面
    /// </summary>
    public bool IsVirtual { get; init; }

    /// <summary>
    /// IPv4 位址
    /// </summary>
    public IReadOnlyList<string> IPv4Addresses { get; init; }
        = Array.Empty<string>();

    /// <summary>
    /// IPv6 位址
    /// </summary>
    public IReadOnlyList<string> IPv6Addresses { get; init; }
        = Array.Empty<string>();

    /// <summary>
    /// MAC Address
    /// </summary>
    public string MacAddress { get; init; } = string.Empty;

    /// <summary>
    /// Default Gateway
    /// </summary>
    public IReadOnlyList<string> Gateways { get; init; }
        = Array.Empty<string>();

    /// <summary>
    /// DNS Server
    /// </summary>
    public IReadOnlyList<string> DnsServers { get; init; }
        = Array.Empty<string>();

    /// <summary>
    /// Link Speed（Mbps）
    /// </summary>
    public long? LinkSpeedMbps { get; init; }

    /// <summary>
    /// 最大傳輸單元
    /// </summary>
    public int? Mtu { get; init; }
}