namespace NetCheckMonitor.Core.Services.Mac;

using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Versioning;
using NetCheckMonitor.Core.Models.SystemInfo;

[SupportedOSPlatform("macos")]
public sealed class MacNetworkInfoProvider
{
    public IReadOnlyList<NetworkAdapterInfo> GetNetworkAdapters()
    {
        if (!OperatingSystem.IsMacOS())
        {
            throw new PlatformNotSupportedException();
        }

        List<NetworkAdapterInfo> adapters = [];

        Dictionary<string, string> hardwarePorts = GetHardwarePorts();

        NetworkInterface[] networkInterfaces =
            NetworkInterface.GetAllNetworkInterfaces();

        string? defaultGateway = networkInterfaces
            .SelectMany(networkInterface =>
                networkInterface.GetIPProperties().GatewayAddresses)
            .Select(gateway => gateway.Address.ToString())
            .FirstOrDefault(address =>
                !string.IsNullOrWhiteSpace(address) &&
                address != "0.0.0.0" &&
                address != "::");

        int index = 0;

        foreach (NetworkInterface networkInterface in networkInterfaces)
        {
            IPInterfaceProperties ipProperties =
                networkInterface.GetIPProperties();

            string[] ipv4Addresses = ipProperties.UnicastAddresses
                .Where(address =>
                    address.Address.AddressFamily ==
                    AddressFamily.InterNetwork)
                .Select(address => address.Address.ToString())
                .ToArray();

            string[] ipv6Addresses = ipProperties.UnicastAddresses
                .Where(address =>
                    address.Address.AddressFamily ==
                    AddressFamily.InterNetworkV6)
                .Select(address => address.Address.ToString())
                .ToArray();

            string[] gateways = ipProperties.GatewayAddresses
                .Select(gateway => gateway.Address.ToString())
                .Where(address =>
                    address != "0.0.0.0" &&
                    address != "::")
                .ToArray();

            string[] dnsServers = ipProperties.DnsAddresses
                .Select(address => address.ToString())
                .ToArray();

            int? mtu = GetMtu(
                networkInterface,
                ipProperties);

            string adapterText =
                $"{networkInterface.Name} {networkInterface.Description}";

            bool isVirtual = IsVirtualAdapter(
                networkInterface,
                adapterText);

            bool isDefault =
                defaultGateway is not null &&
                gateways.Contains(
                    defaultGateway,
                    StringComparer.OrdinalIgnoreCase);

            long? linkSpeedMbps =
                networkInterface.Speed > 0
                    ? networkInterface.Speed / 1_000_000
                    : null;

            adapters.Add(
                new NetworkAdapterInfo
                {
                    Index = index,
                    Id = networkInterface.Id,
                    Name = networkInterface.Name,
                    Description = networkInterface.Description,
                    Type = hardwarePorts.TryGetValue(networkInterface.Name,out string? hardwarePort)? hardwarePort: GetAdapterType(networkInterface),
                    IsUp =
                        networkInterface.OperationalStatus ==
                        OperationalStatus.Up,
                    IsDefault = isDefault,
                    IsVirtual = isVirtual,
                    IPv4Addresses = ipv4Addresses,
                    IPv6Addresses = ipv6Addresses,
                    MacAddress = GetMacAddress(networkInterface),
                    Gateways = gateways,
                    DnsServers = dnsServers,
                    LinkSpeedMbps = linkSpeedMbps,
                    Mtu = mtu
                });

            index++;
        }

        return adapters;
    }

    private static int? GetMtu(
        NetworkInterface networkInterface,
        IPInterfaceProperties ipProperties)
    {
        if (networkInterface.Supports(
                NetworkInterfaceComponent.IPv4))
        {
            return ipProperties.GetIPv4Properties()?.Mtu;
        }

        if (networkInterface.Supports(
                NetworkInterfaceComponent.IPv6))
        {
            return ipProperties.GetIPv6Properties()?.Mtu;
        }

        return null;
    }

    private static string GetMacAddress(
        NetworkInterface networkInterface)
    {
        byte[] addressBytes = networkInterface
            .GetPhysicalAddress()
            .GetAddressBytes();

        return string.Join(
            ":",
            addressBytes.Select(value =>
                value.ToString("X2")));
    }

    private static bool IsVirtualAdapter(
        NetworkInterface networkInterface,
        string adapterText)
    {
        if (networkInterface.NetworkInterfaceType is
            NetworkInterfaceType.Loopback or
            NetworkInterfaceType.Tunnel)
        {
            return true;
        }

        string name = networkInterface.Name;

        string[] virtualPrefixes =
        [
            "lo",
            "utun",
            "awdl",
            "llw",
            "bridge",
            "gif",
            "stf",
            "p2p",
            "anpi",
            "ap"
        ];

        if (virtualPrefixes.Any(prefix =>
                name.StartsWith(
                    prefix,
                    StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        string[] virtualKeywords =
        [
            "virtual",
            "vmware",
            "virtualbox",
            "vpn",
            "tunnel",
            "bridge"
        ];

        return virtualKeywords.Any(keyword =>
            adapterText.Contains(
                keyword,
                StringComparison.OrdinalIgnoreCase));
    }

    private static string GetAdapterType(
        NetworkInterface networkInterface)
    {
        string name = networkInterface.Name;

        if (name.StartsWith(
                "utun",
                StringComparison.OrdinalIgnoreCase))
        {
            return "VPN";
        }

        if (name.StartsWith(
                "bridge",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Bridge";
        }

        if (name.StartsWith(
                "awdl",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Apple Wireless Direct Link";
        }

        if (name.StartsWith(
                "lo",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Loopback";
        }

        return networkInterface.NetworkInterfaceType switch
        {
            NetworkInterfaceType.Wireless80211 => "Wi-Fi",
            NetworkInterfaceType.Ethernet => "Ethernet",
            NetworkInterfaceType.GigabitEthernet => "Ethernet",
            NetworkInterfaceType.FastEthernetFx => "Ethernet",
            NetworkInterfaceType.FastEthernetT => "Ethernet",
            NetworkInterfaceType.Tunnel => "VPN",
            NetworkInterfaceType.Loopback => "Loopback",
            _ => networkInterface.NetworkInterfaceType.ToString()
        };
    }
    private static Dictionary<string, string> GetHardwarePorts()
    {
        Dictionary<string, string> result = [];

        try
        {
            string output = MacCommandRunner.Run(
                "/usr/sbin/networksetup",
                "-listallhardwareports");

            string? currentPort = null;

            foreach (string rawLine in output.Split(
                         '\n',
                         StringSplitOptions.RemoveEmptyEntries))
            {
                string line = rawLine.Trim();

                if (line.StartsWith(
                        "Hardware Port: ",
                        StringComparison.Ordinal))
                {
                    currentPort =
                        line["Hardware Port: ".Length..].Trim();

                    continue;
                }

                if (line.StartsWith(
                        "Device: ",
                        StringComparison.Ordinal)
                    && currentPort is not null)
                {
                    string device =
                        line["Device: ".Length..].Trim();

                    if (!string.IsNullOrWhiteSpace(device))
                    {
                        result[device] = currentPort;
                    }

                    currentPort = null;
                }
            }
        }
        catch
        {
            // 無法取得 hardware port 時，
            // 會回退使用 NetworkInterfaceType。
        }

        return result;
    }
}