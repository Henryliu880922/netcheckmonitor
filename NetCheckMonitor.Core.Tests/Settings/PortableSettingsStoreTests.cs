using NetCheckMonitor.Core;
using Xunit;

namespace NetCheckMonitor.Core.Tests;

public sealed class PortableSettingsStoreTests
{
    [Fact]
    public void SaveAndLoad_ReturnsSameSettings()
    {
        var settings = new MonitorTargetSettings
        {
            UseCustomTargets = true,
            CustomTargets = new List<string>
            {
                "https://example.com/"
            },
            AutoStartMonitoring = true,
            PreventSleepWhileMonitoring = false
        };

        string path = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid() + ".json");

        try
        {
            MonitorSettingsStore.SaveToPath(path, settings);

            MonitorTargetSettings loaded =
                MonitorSettingsStore.LoadFromPath(path);

            Assert.True(loaded.UseCustomTargets);
            Assert.True(loaded.AutoStartMonitoring);
            Assert.False(loaded.PreventSleepWhileMonitoring);

            Assert.Single(loaded.CustomTargets);
            Assert.Equal(
                "https://example.com/",
                loaded.CustomTargets[0]);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            string tempPath = path + ".tmp";

            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }
    [Fact]
    public void LoadFromPath_ReturnsDefaultSettings_WhenFileDoesNotExist()
    {
        string path = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid() + ".json");

        MonitorTargetSettings settings =
            MonitorSettingsStore.LoadFromPath(path);

        Assert.False(settings.UseCustomTargets);
        Assert.Empty(settings.CustomTargets);
        Assert.True(settings.PreventSleepWhileMonitoring);
    }
    [Fact]
    public void LoadFromPath_ReturnsDefaultSettings_WhenJsonIsInvalid()
    {
        string path = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid() + ".json");

        try
        {
            File.WriteAllText(path, "{ invalid json");

            MonitorTargetSettings settings =
                MonitorSettingsStore.LoadFromPath(path);

            Assert.False(settings.UseCustomTargets);
            Assert.Empty(settings.CustomTargets);
            Assert.True(settings.PreventSleepWhileMonitoring);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
    [Fact]
    public void SaveToPath_CreatesDirectory_WhenDirectoryDoesNotExist()
    {
        string root = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString());

        string path = Path.Combine(
            root,
            "settings",
            "monitor-settings.json");

        try
        {
            var settings = new MonitorTargetSettings
            {
                AutoStartMonitoring = true
            };

            MonitorSettingsStore.SaveToPath(path, settings);

            Assert.True(Directory.Exists(
                Path.GetDirectoryName(path)!));

            Assert.True(File.Exists(path));

            MonitorTargetSettings loaded =
                MonitorSettingsStore.LoadFromPath(path);

            Assert.True(loaded.AutoStartMonitoring);
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }
    [Fact]
    public void LoadFromPath_KeepsOnlyFirstThreeCustomTargets()
    {
        string path = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid() + ".json");

        try
        {
            var settings = new MonitorTargetSettings
            {
                UseCustomTargets = true,
                CustomTargets = new List<string>
            {
                "https://one.example",
                "https://two.example",
                "https://three.example",
                "https://four.example"
            }
            };

            MonitorSettingsStore.SaveToPath(path, settings);

            MonitorTargetSettings loaded =
                MonitorSettingsStore.LoadFromPath(path);

            Assert.Equal(3, loaded.CustomTargets.Count);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
    [Fact]
    public void LoadFromPath_RemovesDuplicateCustomTargetsIgnoringCase()
    {
        string path = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid() + ".json");

        try
        {
            var settings = new MonitorTargetSettings
            {
                UseCustomTargets = true,
                CustomTargets = new List<string>
            {
                "https://example.com",
                "https://EXAMPLE.com"
            }
            };

            MonitorSettingsStore.SaveToPath(path, settings);

            MonitorTargetSettings loaded =
                MonitorSettingsStore.LoadFromPath(path);

            Assert.Single(loaded.CustomTargets);
            Assert.Equal(
                "https://example.com/",
                loaded.CustomTargets[0],
                ignoreCase: true);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
    [Fact]
    public void LoadFromPath_RemovesInvalidCustomTargets()
    {
        string path = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid() + ".json");

        try
        {
            var settings = new MonitorTargetSettings
            {
                UseCustomTargets = true,
                CustomTargets = new List<string>
            {
                "https://example.com",
                "not a valid target"
            }
            };

            MonitorSettingsStore.SaveToPath(path, settings);

            MonitorTargetSettings loaded =
                MonitorSettingsStore.LoadFromPath(path);

            Assert.Single(loaded.CustomTargets);
            Assert.Equal(
                "https://example.com/",
                loaded.CustomTargets[0]);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}