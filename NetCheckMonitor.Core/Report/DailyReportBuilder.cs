namespace NetCheckMonitor.Core.Report;

public sealed class DailyReportBuilder
{
    public DailyReportData Build(MonitoringSession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        TimeSpan total = session.End - session.Start;

        TimeSpan paused = session.PausePeriods
            .Aggregate(
                TimeSpan.Zero,
                (sum, pause) => sum + (pause.End - pause.Start));

        TimeSpan outage = session.Outages
            .Aggregate(
                TimeSpan.Zero,
                (sum, item) => sum + (item.End - item.Start));
        TimeSpan longestOutage = session.Outages.Count == 0
            ? TimeSpan.Zero
            : session.Outages.Max(
                item => item.End - item.Start);
        TimeSpan effective = total - paused;

        double availability = effective <= TimeSpan.Zero
            ? 0d
            : (effective - outage).TotalSeconds
            / effective.TotalSeconds
            * 100d;

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

            Records = session.Records.ToList(),
            EventNotes = session.EventNotes.ToList()
        };
    }
}