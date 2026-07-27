namespace NetCheckMonitor.Core.Services.Mac;

using System.Globalization;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Text.RegularExpressions;
using NetCheckMonitor.Core.Models.SystemInfo;

[SupportedOSPlatform("macos")]
internal static class MacDisplayInfoProvider
{
    public static IReadOnlyList<DisplayInfo> GetDisplays()
    {
        if (!OperatingSystem.IsMacOS())
        {
            throw new PlatformNotSupportedException();
        }

        try
        {
            string json = MacCommandRunner.Run(
                "/usr/sbin/system_profiler",
                "SPDisplaysDataType",
                "-json");

            using JsonDocument document = JsonDocument.Parse(json);

            if (!document.RootElement.TryGetProperty(
                    "SPDisplaysDataType",
                    out JsonElement graphicsDevices)
                || graphicsDevices.ValueKind != JsonValueKind.Array)
            {
                return [];
            }

            List<DisplayInfo> displays = [];

            foreach (JsonElement graphicsDevice
                     in graphicsDevices.EnumerateArray())
            {
                if (!graphicsDevice.TryGetProperty(
                        "spdisplays_ndrvs",
                        out JsonElement displayItems)
                    || displayItems.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                foreach (JsonElement displayItem
                         in displayItems.EnumerateArray())
                {
                    string name = GetString(
                        displayItem,
                        "_name");

                    string pixelText = GetString(
                        displayItem,
                        "_spdisplays_pixels");

                    string resolutionText = GetString(
                        displayItem,
                        "_spdisplays_resolution");

                    (int width, int height) =
                        ParseResolution(pixelText);

                    displays.Add(
                        new DisplayInfo
                        {
                            Name = name,
                            Manufacturer = string.Empty,
                            Model = name,
                            SerialNumber = string.Empty,
                            Width = width,
                            Height = height,
                            RefreshRate =
                                ParseRefreshRate(resolutionText),
                            IsPrimary =
                                GetString(
                                    displayItem,
                                    "spdisplays_main")
                                .Equals(
                                    "spdisplays_yes",
                                    StringComparison.OrdinalIgnoreCase)
                        });
                }
            }

            return displays;
        }
        catch
        {
            return [];
        }
    }

    private static string GetString(
        JsonElement element,
        string propertyName)
    {
        if (!element.TryGetProperty(
                propertyName,
                out JsonElement value))
        {
            return string.Empty;
        }

        return value.ValueKind == JsonValueKind.String
            ? value.GetString() ?? string.Empty
            : value.ToString();
    }

    private static (int Width, int Height) ParseResolution(
        string text)
    {
        Match match = Regex.Match(
            text,
            @"(?<width>\d+)\s*x\s*(?<height>\d+)",
            RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            return (0, 0);
        }

        int.TryParse(
            match.Groups["width"].Value,
            out int width);

        int.TryParse(
            match.Groups["height"].Value,
            out int height);

        return (width, height);
    }

    private static double? ParseRefreshRate(
        string text)
    {
        Match match = Regex.Match(
            text,
            @"@\s*(?<rate>\d+(?:\.\d+)?)\s*Hz",
            RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            return null;
        }

        return double.TryParse(
            match.Groups["rate"].Value,
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out double refreshRate)
            ? refreshRate
            : null;
    }
}