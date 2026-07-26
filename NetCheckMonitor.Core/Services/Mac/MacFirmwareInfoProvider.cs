namespace NetCheckMonitor.Core.Services.Mac;

using System.Globalization;
using System.Runtime.Versioning;
using System.Text.Json;
using NetCheckMonitor.Core.Models.SystemInfo;

[SupportedOSPlatform("macos")]
internal static class MacFirmwareInfoProvider
{
    public static FirmwareInfo GetFirmwareInfo()
    {
        if (!OperatingSystem.IsMacOS())
        {
            throw new PlatformNotSupportedException();
        }

        string output = MacCommandRunner.Run(
            "/usr/sbin/system_profiler",
            "SPHardwareDataType",
            "-json");

        if (string.IsNullOrWhiteSpace(output))
        {
            return new FirmwareInfo();
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(output);

            if (!document.RootElement.TryGetProperty(
                    "SPHardwareDataType",
                    out JsonElement array) ||
                array.ValueKind != JsonValueKind.Array ||
                array.GetArrayLength() == 0)
            {
                return new FirmwareInfo();
            }

            JsonElement hardware = array[0];

            string version = GetString(
                hardware,
                "boot_rom_version");
            if (string.IsNullOrWhiteSpace(version))
            {
                version = GetString(

                    hardware,

                    "os_loader_version");
            }
            return new FirmwareInfo
            {
                // Apple 沒有 BIOS / UEFI，統一表示為 Apple Boot ROM
                Type = "Apple Boot ROM",

                Version = version,

                Manufacturer = "Apple",

                // system_profiler 無提供
                ReleaseDate = null,

                // Apple Silicon 一律採用 EFI 韌體架構啟動
                IsUefi = true
            };
        }
        catch (JsonException)
        {
            return new FirmwareInfo();
        }
    }

    private static string GetString(
        JsonElement element,
        string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out JsonElement value))
        {
            return string.Empty;
        }

        return value.ValueKind == JsonValueKind.String
            ? value.GetString() ?? string.Empty
            : value.ToString();
    }
}