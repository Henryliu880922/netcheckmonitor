using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;

namespace NetCheckMonitor.Core;

public sealed class MonitorTargetSettings
{
    public bool UseCustomTargets { get; set; }
    public List<string> CustomTargets { get; set; } = new();
    public bool AutoStartWindows { get; set; }
    public bool AutoStartMonitoring { get; set; }
    public bool AdvancedDiagnosticsEnabled { get; set; }
    public bool PreventSleepWhileMonitoring { get; set; } = true;
    public bool PreventShutdownWhileMonitoring { get; set; }
}

public static class MonitorSettingsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public static MonitorTargetSettings LoadFromPath(string path)
    {
        try
        {
            if (!File.Exists(path))
                return DefaultSettings();

            string json = File.ReadAllText(path, Encoding.UTF8);
            bool hasSleepSetting =
                json.Contains("\"PreventSleepWhileMonitoring\"", StringComparison.Ordinal);

            MonitorTargetSettings? value =
                JsonSerializer.Deserialize<MonitorTargetSettings>(json, JsonOptions);

            return NormalizeLoaded(value, hasSleepSetting);
        }
        catch
        {
            return DefaultSettings();
        }
    }

    public static void SaveToPath(string path, MonitorTargetSettings value)
    {
        ArgumentNullException.ThrowIfNull(value);

        string? directory = Path.GetDirectoryName(path);

        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        string json = JsonSerializer.Serialize(value, JsonOptions);
        string temp = path + ".tmp";

        File.WriteAllText(temp, json, new UTF8Encoding(false));

        try
        {
            if (File.Exists(path))
                File.Move(temp, path, true);
            else
                File.Move(temp, path);
        }
        catch
        {
            if (File.Exists(path))
                File.Delete(path);

            File.Move(temp, path);
        }
    }

    public static string[] GetEffectiveTargets(
        MonitorTargetSettings? settings,
        string[] builtInTargets)
    {
        ArgumentNullException.ThrowIfNull(builtInTargets);

        if (settings?.UseCustomTargets == true &&
            settings.CustomTargets.Count > 0)
        {
            return settings.CustomTargets.ToArray();
        }

        return (string[])builtInTargets.Clone();
    }

    public static bool TryNormalizeTarget(
        string? input,
        out string? normalized,
        out string? error)
    {
        normalized = null;
        error = null;

        string value = (input ?? string.Empty).Trim();

        if (value.Length == 0)
        {
            error = "目標不可空白。";
            return false;
        }

        if (value.IndexOfAny(new[] { '\r', '\n', '\t' }) >= 0)
        {
            error = "目標格式無效。";
            return false;
        }

        string candidate = value;

        if (!value.Contains("://", StringComparison.Ordinal))
        {
            if (IPAddress.TryParse(value, out IPAddress? address))
            {
                candidate =
                    address.AddressFamily ==
                    System.Net.Sockets.AddressFamily.InterNetworkV6
                        ? $"http://[{value}]/"
                        : $"http://{value}/";
            }
            else
            {
                candidate = $"https://{value}";
            }
        }

        if (!Uri.TryCreate(candidate, UriKind.Absolute, out Uri? uri) ||
            string.IsNullOrWhiteSpace(uri.Host))
        {
            error = "請輸入有效的網站或 IP。";
            return false;
        }

        if (!uri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) &&
            !uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
        {
            error = "僅支援 HTTP 或 HTTPS 目標。";
            return false;
        }

        if (!string.IsNullOrEmpty(uri.UserInfo))
        {
            error = "目標不可包含帳號或密碼。";
            return false;
        }

        normalized = uri.AbsoluteUri;
        return true;
    }

    private static MonitorTargetSettings DefaultSettings()
    {
        return new MonitorTargetSettings
        {
            UseCustomTargets = false,
            CustomTargets = new List<string>(),
            PreventSleepWhileMonitoring = true
        };
    }

    private static MonitorTargetSettings NormalizeLoaded(
        MonitorTargetSettings? value,
        bool hasSleepSetting)
    {
        if (value is null)
            return DefaultSettings();

        if (!hasSleepSetting)
            value.PreventSleepWhileMonitoring = true;

        value.CustomTargets ??= new List<string>();

        var valid = new List<string>();

        foreach (string target in value.CustomTargets)
        {
            if (valid.Count >= 3)
                break;

            if (TryNormalizeTarget(target, out string? normalized, out _) &&
                normalized is not null &&
                !ContainsIgnoreCase(valid, normalized))
            {
                valid.Add(normalized);
            }
        }

        value.CustomTargets = valid;

        if (value.UseCustomTargets && valid.Count == 0)
            value.UseCustomTargets = false;

        return value;
    }

    private static bool ContainsIgnoreCase(
        List<string> values,
        string candidate)
    {
        foreach (string value in values)
        {
            if (string.Equals(
                value,
                candidate,
                StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
