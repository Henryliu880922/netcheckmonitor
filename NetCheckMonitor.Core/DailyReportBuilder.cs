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

        TimeSpan effective = total - paused;
        return new DailyReportData
        {
            MachineName = session.MachineName,
            MachineId = session.MachineId,
            Day = session.Start.Date,
            Effective = effective,
            Records = session.Records.ToList(),
            EventNotes = session.EventNotes.ToList()
        };
    }
}