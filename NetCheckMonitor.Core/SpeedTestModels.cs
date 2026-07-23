using System;

namespace NetCheckMonitor.Core;

public enum SpeedTestLevel
{
    Quick,
    Standard,
    Full
}

public sealed class SpeedTestOptions
{
    public bool ScheduledEnabled { get; set; }

    public int IntervalHours { get; set; } = 24;

    public string Level { get; set; } = SpeedTestLevel.Standard.ToString();

    public bool AllowMeteredNetwork { get; set; }

    public DateTime LastScheduledRunUtc { get; set; }

    public DateTime LastAttemptUtc { get; set; }

    public DateTime ServerCooldownUntilUtc { get; set; }

    public int RateLimitBackoffLevel { get; set; }

    public SpeedTestLevel EffectiveLevel
    {
        get
        {
            return Enum.TryParse(Level, true, out SpeedTestLevel value)
                ? value
                : SpeedTestLevel.Standard;
        }
    }

    public static SpeedTestOptions Defaults()
    {
        return new SpeedTestOptions
        {
            ScheduledEnabled = false,
            IntervalHours = 24,
            Level = SpeedTestLevel.Standard.ToString(),
            AllowMeteredNetwork = false
        };
    }
}
