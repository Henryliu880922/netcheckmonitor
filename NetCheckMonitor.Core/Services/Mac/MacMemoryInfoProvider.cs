namespace NetCheckMonitor.Core.Services.Mac;

using System.Globalization;
using System.Runtime.Versioning;
using NetCheckMonitor.Core.Models.SystemInfo;

[SupportedOSPlatform("macos")]
internal static class MacMemoryInfoProvider
{
    public static MemoryInfo GetMemoryInfo()
    {
        ulong totalBytes = GetTotalBytes();
        ulong availableBytes = GetAvailableBytes();

        if (availableBytes > totalBytes)
        {
            availableBytes = totalBytes;
        }

        ulong usedBytes = totalBytes - availableBytes;

        double usagePercentage = totalBytes > 0
            ? (double)usedBytes / totalBytes * 100
            : 0;

        return new MemoryInfo
        {
            TotalBytes = totalBytes,
            AvailableBytes = availableBytes,
            UsedBytes = usedBytes,
            UsagePercentage = usagePercentage,
            MemoryType = "Unified",
            ModuleCount = null,
        };
    }

    private static ulong GetTotalBytes()
    {
        string value = MacCommandRunner.Run(
            "/usr/sbin/sysctl",
            "-n",
            "hw.memsize");

        return ulong.TryParse(
            value,
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out ulong totalBytes)
            ? totalBytes
            : 0;
    }

    private static ulong GetAvailableBytes()
    {
        string output = MacCommandRunner.Run(
            "/usr/bin/vm_stat");

        if (string.IsNullOrWhiteSpace(output))
        {
            return 0;
        }

        ulong pageSize = ParsePageSize(output);

        if (pageSize == 0)
        {
            return 0;
        }

        ulong freePages = ParsePageCount(output, "Pages free:");
        ulong inactivePages = ParsePageCount(output, "Pages inactive:");
        ulong speculativePages = ParsePageCount(output, "Pages speculative:");

        return (freePages + inactivePages + speculativePages) * pageSize;
    }

    private static ulong ParsePageSize(string output)
    {
        const string marker = "page size of ";

        int markerIndex = output.IndexOf(
            marker,
            StringComparison.OrdinalIgnoreCase);

        if (markerIndex < 0)
        {
            return 0;
        }

        int valueStart = markerIndex + marker.Length;
        int valueEnd = output.IndexOf(' ', valueStart);

        if (valueEnd < 0)
        {
            return 0;
        }

        string value = output[valueStart..valueEnd];

        return ulong.TryParse(
            value,
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out ulong pageSize)
            ? pageSize
            : 0;
    }

    private static ulong ParsePageCount(
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

            string value = trimmedLine[fieldName.Length..]
                .Trim()
                .TrimEnd('.');

            return ulong.TryParse(
                value,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out ulong pageCount)
                ? pageCount
                : 0;
        }

        return 0;
    }
}