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
}