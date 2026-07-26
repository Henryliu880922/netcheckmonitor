namespace NetCheckMonitor.Core.Services.Mac;

using System.Globalization;
using System.Runtime.Versioning;
using System.Text.Json;
using NetCheckMonitor.Core.Models.SystemInfo;

[SupportedOSPlatform("macos")]
internal sealed class MacBatteryInfoProvider
{
    public BatteryInfo GetBatteryInfo()
    {
        if (!OperatingSystem.IsMacOS())
        {
            throw new PlatformNotSupportedException();
        }

        string output = MacCommandRunner.Run(
            "/usr/sbin/system_profiler",
            "SPPowerDataType",
            "-json");

        if (string.IsNullOrWhiteSpace(output))
        {
            return new BatteryInfo
            {
                IsPresent = false,
            };
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(output);

            if (!document.RootElement.TryGetProperty(
                    "SPPowerDataType",
                    out JsonElement powerItems) ||
                powerItems.ValueKind != JsonValueKind.Array)
            {
                return new BatteryInfo
                {
                    IsPresent = false,
                };
            }

            JsonElement? batteryItem = FindItem(
                powerItems,
                "spbattery_information");

            if (batteryItem is null)
            {
                return new BatteryInfo
                {
                    IsPresent = false,
                };
            }

            JsonElement battery = batteryItem.Value;

            double chargePercentage = 0;
            bool isCharging = false;
            int? cycleCount = null;
            double? healthPercentage = null;
            string model = string.Empty;

            if (battery.TryGetProperty(
                    "sppower_battery_charge_info",
                    out JsonElement chargeInfo))
            {
                chargePercentage = GetDouble(
                    chargeInfo,
                    "sppower_battery_state_of_charge");

                isCharging = GetBoolean(
                    chargeInfo,
                    "sppower_battery_is_charging");
            }

            if (battery.TryGetProperty(
                    "sppower_battery_health_info",
                    out JsonElement healthInfo))
            {
                cycleCount = GetNullableInt32(
                    healthInfo,
                    "sppower_battery_cycle_count");

                healthPercentage = GetPercentage(
                    healthInfo,
                    "sppower_battery_health_maximum_capacity");
            }

            if (battery.TryGetProperty(
                    "sppower_battery_model_info",
                    out JsonElement modelInfo))
            {
                model = GetString(
                    modelInfo,
                    "sppower_battery_device_name");
            }

            return new BatteryInfo
            {
                IsPresent = true,
                IsCharging = isCharging,
                ChargePercentage = chargePercentage,

                // system_profiler 在目前的 Apple Silicon macOS
                // 不提供實際容量值，因此先保留為 0。
                DesignCapacity = 0,
                FullChargeCapacity = 0,
                CurrentCapacity = 0,

                HealthPercentage = healthPercentage,
                CycleCount = cycleCount,
                Manufacturer = "Apple",
                Model = model,
                RemainingTime = GetRemainingTime(),
            };
        }
        catch (JsonException)
        {
            return new BatteryInfo
            {
                IsPresent = false,
            };
        }
    }

    private static JsonElement? FindItem(
        JsonElement items,
        string itemName)
    {
        foreach (JsonElement item in items.EnumerateArray())
        {
            if (!item.TryGetProperty(
                    "_name",
                    out JsonElement nameElement))
            {
                continue;
            }

            if (string.Equals(
                    nameElement.GetString(),
                    itemName,
                    StringComparison.Ordinal))
            {
                return item;
            }
        }

        return null;
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

    private static double GetDouble(
        JsonElement element,
        string propertyName)
    {
        if (!element.TryGetProperty(
                propertyName,
                out JsonElement value))
        {
            return 0;
        }

        if (value.ValueKind == JsonValueKind.Number &&
            value.TryGetDouble(out double number))
        {
            return number;
        }

        return double.TryParse(
            value.ToString(),
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out number)
            ? number
            : 0;
    }

    private static int? GetNullableInt32(
        JsonElement element,
        string propertyName)
    {
        if (!element.TryGetProperty(
                propertyName,
                out JsonElement value))
        {
            return null;
        }

        if (value.ValueKind == JsonValueKind.Number &&
            value.TryGetInt32(out int number))
        {
            return number;
        }

        return int.TryParse(
            value.ToString(),
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out number)
            ? number
            : null;
    }

    private static bool GetBoolean(
        JsonElement element,
        string propertyName)
    {
        string value = GetString(element, propertyName);

        return string.Equals(
                   value,
                   "TRUE",
                   StringComparison.OrdinalIgnoreCase) ||
               string.Equals(
                   value,
                   "YES",
                   StringComparison.OrdinalIgnoreCase) ||
               string.Equals(
                   value,
                   "1",
                   StringComparison.Ordinal);
    }

    private static double? GetPercentage(
        JsonElement element,
        string propertyName)
    {
        string value = GetString(element, propertyName)
            .Trim()
            .TrimEnd('%');

        return double.TryParse(
            value,
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out double percentage)
            ? percentage
            : null;
    }

    private static TimeSpan? GetRemainingTime()
    {
        string output = MacCommandRunner.Run(
            "/usr/bin/pmset",
            "-g",
            "batt");

        if (string.IsNullOrWhiteSpace(output))
        {
            return null;
        }

        string[] sections = output.Split(';');

        foreach (string section in sections)
        {
            string value = section.Trim();

            int colonIndex = value.IndexOf(':');

            if (colonIndex <= 0)
            {
                continue;
            }

            string hourText = value[..colonIndex].Trim();
            string minuteText = new(
                value[(colonIndex + 1)..]
                    .TakeWhile(char.IsDigit)
                    .ToArray());

            if (!int.TryParse(
                    hourText,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out int hours) ||
                !int.TryParse(
                    minuteText,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out int minutes))
            {
                continue;
            }

            return TimeSpan.FromHours(hours)
                   + TimeSpan.FromMinutes(minutes);
        }

        return null;
    }
}