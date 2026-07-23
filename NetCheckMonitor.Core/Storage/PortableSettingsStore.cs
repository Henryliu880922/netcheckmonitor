using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace NetCheckMonitor.Core;

public sealed class PortableAppSettings
{
    public int FormatVersion { get; set; } = 1;

    public string? Language { get; set; }

    public bool? CloseToTrayNoticeShown { get; set; }

    public string? CloudBackupSchedule { get; set; }

    public MonitorTargetSettings? Monitor { get; set; }
}

public static class PortableSettingsStore
{
    private static readonly object Sync = new();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public static string SettingsPath
    {
        get
        {
            string? overridePath =
                Environment.GetEnvironmentVariable("NETCHECK_PORTABLE_SETTINGS");

            return string.IsNullOrWhiteSpace(overridePath)
                ? Path.Combine(GetApplicationDataDirectory(), "settings.json")
                : overridePath;
        }
    }

    public static string SessionPath
    {
        get
        {
            string? overridePath =
                Environment.GetEnvironmentVariable("NETCHECK_SESSION_STATE");

            return string.IsNullOrWhiteSpace(overridePath)
                ? Path.Combine(GetApplicationDataDirectory(), "session.json")
                : overridePath;
        }
    }

    public static string CloudPath
    {
        get
        {
            string? overridePath =
                Environment.GetEnvironmentVariable("NETCHECK_CLOUD_SETTINGS");

            return string.IsNullOrWhiteSpace(overridePath)
                ? Path.Combine(GetApplicationDataDirectory(), "cloud.dat")
                : overridePath;
        }
    }

    public static string? LoadLanguage()
    {
        lock (Sync)
        {
            return ReadSettings(SettingsPath).Language;
        }
    }

    public static void SaveLanguage(string? language)
    {
        Update(settings => settings.Language = language);
    }

    public static bool? LoadCloseNoticeShown()
    {
        lock (Sync)
        {
            return ReadSettings(SettingsPath).CloseToTrayNoticeShown;
        }
    }

    public static void SaveCloseNoticeShown(bool shown)
    {
        Update(settings => settings.CloseToTrayNoticeShown = shown);
    }

    public static MonitorTargetSettings? LoadMonitorSettings()
    {
        lock (Sync)
        {
            return ReadSettings(SettingsPath).Monitor;
        }
    }

    public static void SaveMonitorSettings(MonitorTargetSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        Update(value => value.Monitor = settings);
    }

    public static string? LoadCloudBackupSchedule()
    {
        lock (Sync)
        {
            return ReadSettings(SettingsPath).CloudBackupSchedule;
        }
    }

    public static void SaveCloudBackupSchedule(string? schedule)
    {
        Update(settings => settings.CloudBackupSchedule = schedule);
    }

    private static void Update(Action<PortableAppSettings> change)
    {
        lock (Sync)
        {
            PortableAppSettings value = ReadSettings(SettingsPath);

            change(value);
            value.FormatVersion = 1;

            WriteSettings(SettingsPath, value);
        }
    }

    private static PortableAppSettings ReadSettings(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                return new PortableAppSettings();
            }

            string json = File.ReadAllText(path, Encoding.UTF8);

            PortableAppSettings? value =
                JsonSerializer.Deserialize<PortableAppSettings>(
                    json,
                    JsonOptions);

            return value ?? new PortableAppSettings();
        }
        catch
        {
            return new PortableAppSettings();
        }
    }

    private static void WriteSettings(
        string path,
        PortableAppSettings value)
    {
        string? directory = Path.GetDirectoryName(path);

        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string json = JsonSerializer.Serialize(value, JsonOptions);
        string tempPath = path + ".tmp";

        File.WriteAllText(
            tempPath,
            json,
            new UTF8Encoding(false));

        File.Move(tempPath, path, true);
    }

    private static string GetApplicationDataDirectory()
    {
        string baseDirectory =
            Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData);

        return Path.Combine(
            baseDirectory,
            "NetCheckMonitor");
    }
}