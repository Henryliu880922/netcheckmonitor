namespace NetCheckMonitor.Core.Services.Mac;

using System.Globalization;
using System.Runtime.Versioning;
using NetCheckMonitor.Core.Models.SystemInfo;

[SupportedOSPlatform("macos")]
internal static class MacStorageInfoProvider
{
    public static IReadOnlyList<StorageInfo> GetStorageInfo()
    {
        string systemVolumeInfo = MacCommandRunner.Run(
            "/usr/sbin/diskutil",
            "info",
            "/");

        string physicalDiskName = GetValue(
            systemVolumeInfo,
            "APFS Physical Store:")
            .Split('s')[0];

        if (string.IsNullOrWhiteSpace(physicalDiskName))
        {
            physicalDiskName = "disk0";
        }

        string physicalDiskInfo = MacCommandRunner.Run(
            "/usr/sbin/diskutil",
            "info",
            physicalDiskName);

        ulong totalBytes = GetByteValue(
            systemVolumeInfo,
            "Container Total Space:");

        ulong availableBytes = GetByteValue(
            systemVolumeInfo,
            "Container Free Space:");

        ulong usedBytes = totalBytes >= availableBytes
            ? totalBytes - availableBytes
            : 0;

        bool isSolidState = string.Equals(
            GetValue(physicalDiskInfo, "Solid State:"),
            "Yes",
            StringComparison.OrdinalIgnoreCase);

        bool isRemovable = !string.Equals(
            GetValue(physicalDiskInfo, "Removable Media:"),
            "Fixed",
            StringComparison.OrdinalIgnoreCase);

        return
        [
            new StorageInfo
            {
                DeviceName = physicalDiskName,
                Manufacturer = "Apple",
                Model = GetValue(
                    physicalDiskInfo,
                    "Device / Media Name:"),

                SerialNumber = string.Empty,

                BusType = GetValue(
                    physicalDiskInfo,
                    "Protocol:"),

                MediaType = isSolidState
                    ? "SSD"
                    : GetValue(
                        physicalDiskInfo,
                        "Media Type:"),

                FirmwareVersion = string.Empty,
                TotalBytes = totalBytes,
                AvailableBytes = availableBytes,
                UsedBytes = usedBytes,
                HealthPercentage = null,
                IsSystemDisk = true,
                IsRemovable = isRemovable,
            },
        ];
    }

    private static string GetValue(
        string output,
        string fieldName)
    {
        foreach (string line in output.Split('\n'))
        {
            string trimmedLine = line.Trim();

            if (!trimmedLine.StartsWith(
                    fieldName,
                    StringComparison.Ordinal))
            {
                continue;
            }

            return trimmedLine[fieldName.Length..].Trim();
        }

        return string.Empty;
    }

    private static ulong GetByteValue(
        string output,
        string fieldName)
    {
        string value = GetValue(output, fieldName);

        int openParenthesisIndex = value.IndexOf('(');
        int bytesIndex = value.IndexOf(
            " Bytes",
            StringComparison.Ordinal);

        if (openParenthesisIndex < 0 ||
            bytesIndex <= openParenthesisIndex)
        {
            return 0;
        }

        string bytesValue = value[
            (openParenthesisIndex + 1)..bytesIndex];

        return ulong.TryParse(
            bytesValue,
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out ulong result)
            ? result
            : 0;
    }
}