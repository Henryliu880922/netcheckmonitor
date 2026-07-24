namespace NetCheckMonitor.Core.Report;

public sealed class DailyReportBuilder
{
    public DailyReportData Build(MonitoringSession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        TimeSpan effective = CalculateEffectiveTime(session);
        TimeSpan outage = CalculateTotalOutage(session);
        TimeSpan longestOutage = CalculateLongestOutage(session);
        double availability = CalculateAvailability(effective, outage);
        double averageLatency = CalculateAverageLatency(session);
        long maximumLatency = CalculateMaximumLatency(session);

        return new DailyReportData
        {
            MachineName = session.MachineName,
            MachineId = session.MachineId,
            Day = session.Start.Date,
            Effective = effective,
            Outage = outage,
            LongestOutage = longestOutage,
            OutageCount = session.Outages.Count,
            Availability = availability,
            CheckCount = session.Records.Count,
            OfflineCount = session.Records.Count(
                record => string.Equals(
                record.Status,
                "OFFLINE",
                StringComparison.OrdinalIgnoreCase)),
            AverageLatency = averageLatency,
            MaximumLatency = maximumLatency,
            Records = session.Records.ToList(),
            EventNotes = session.EventNotes.ToList()
        };
    }

    private static TimeSpan CalculateEffectiveTime(
        MonitoringSession session)
    {
        TimeSpan total = session.End - session.Start;

        TimeSpan paused = session.PausePeriods
            .Aggregate(
                TimeSpan.Zero,
                (sum, pause) => sum + (pause.End - pause.Start));

        return total - paused;
    }

    private static TimeSpan CalculateTotalOutage(
        MonitoringSession session)
    {
        return session.Outages
            .Aggregate(
                TimeSpan.Zero,
                (sum, outage) => sum + (outage.End - outage.Start));
    }

    private static TimeSpan CalculateLongestOutage(
        MonitoringSession session)
    {
        return session.Outages.Count == 0
            ? TimeSpan.Zero
            : session.Outages.Max(
                outage => outage.End - outage.Start);
    }

    private static double CalculateAvailability(
        TimeSpan effective,
        TimeSpan outage)
    {
        return effective <= TimeSpan.Zero
            ? 0d
            : (effective - outage).TotalSeconds
                / effective.TotalSeconds
                * 100d;
    }
    private static double CalculateAverageLatency(
    MonitoringSession session)
    {
        var onlineRecords = session.Records
            .Where(record => record.Online);

        if (!onlineRecords.Any())
        {
            return 0d;
        }

        return onlineRecords.Average(
            record => (double)record.Latency);
    }
    private static long CalculateMaximumLatency(
    MonitoringSession session)
    {
        var onlineRecords = session.Records
            .Where(record => record.Online);

        if (!onlineRecords.Any())
        {
            return 0;
        }

        return onlineRecords.Max(
            record => record.Latency);
    }
}