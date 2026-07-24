namespace NetCheckMonitor.Core.Report;

public sealed class DailyReportBuilder
{
    public DailyReportData Build(MonitoringSession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        return new DailyReportData
        {
            MachineName = session.MachineName,
            MachineId = session.MachineId,
            Day = session.Start.Date,
            Records = session.Records.ToList(),
            EventNotes = session.EventNotes.ToList()
        };
    }
}