using System.Net.NetworkInformation;
using NetCheckMonitor.Core.Models.SystemInfo;

namespace NetCheckMonitor.Core.Services.Windows;

public sealed class WindowsNetworkInfoProvider
{
    public IReadOnlyList<NetworkAdapterInfo> GetNetworkAdapters()
    {
        List<NetworkAdapterInfo> adapters = [];

        int index = 0;

        string? defaultGateway = NetworkInterface.GetAllNetworkInterfaces().SelectMany(networkInterface => networkInterface.GetIPProperties().GatewayAddresses).Select(gateway => gateway.Address.ToString()).FirstOrDefault(address => !string.IsNullOrWhiteSpace(address));

        foreach (NetworkInterface networkInterface
                 in NetworkInterface.GetAllNetworkInterfaces())
        {
            IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
            string[] ipv4Addresses = ipProperties.UnicastAddresses.Where(address => address.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).Select(address => address.Address.ToString()).ToArray();
            string[] ipv6Addresses = ipProperties.UnicastAddresses.Where(address => address.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6).Select(address => address.Address.ToString()).ToArray();
            string[] gateways = ipProperties.GatewayAddresses.Select(gateway => gateway.Address.ToString()).ToArray();
            string[] dnsServers = ipProperties.DnsAddresses.Select(address => address.ToString()).ToArray();

            int? mtu = null;
            if (networkInterface.Supports(NetworkInterfaceComponent.IPv4))
            {
                mtu = ipProperties.GetIPv4Properties()?.Mtu;
            }
            else if (networkInterface.Supports(NetworkInterfaceComponent.IPv6))
            {
                mtu = ipProperties.GetIPv6Properties()?.Mtu;
            }

            string adapterText =
    $"{networkInterface.Name} {networkInterface.Description}";

            bool isVirtual =
                networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback
                || adapterText.Contains(
                    "virtual",
                    StringComparison.OrdinalIgnoreCase)
                || adapterText.Contains(
                    "hyper-v",
                    StringComparison.OrdinalIgnoreCase)
                || adapterText.Contains(
                    "vmware",
                    StringComparison.OrdinalIgnoreCase)
                || adapterText.Contains(
                    "virtualbox",
                    StringComparison.OrdinalIgnoreCase)
                || adapterText.Contains(
                    "vpn",
                    StringComparison.OrdinalIgnoreCase)
                || adapterText.Contains(
                    "tunnel",
                    StringComparison.OrdinalIgnoreCase);

            bool isDefault = gateways.Contains(defaultGateway);

            adapters.Add(
                new NetworkAdapterInfo
                {
                    Index = index,
                    Id = networkInterface.Id,
                    Name = networkInterface.Name,
                    Description = networkInterface.Description,
                    Type = networkInterface.NetworkInterfaceType.ToString(),
                    IsUp = networkInterface.OperationalStatus == OperationalStatus.Up,
                    IPv4Addresses = ipv4Addresses,
                    IPv6Addresses = ipv6Addresses,
                    Gateways = gateways,
                    DnsServers = dnsServers,
                    Mtu = mtu,
                    IsDefault = isDefault,
                    IsVirtual = isVirtual,
                    LinkSpeedMbps = networkInterface.Speed > 0 ? networkInterface.Speed / 1_000_000 : null,
                    MacAddress = string.Join(":", networkInterface.GetPhysicalAddress().GetAddressBytes().Select(value => value.ToString("X2")))
                });
            index++;
        }

        return adapters;
    }
}