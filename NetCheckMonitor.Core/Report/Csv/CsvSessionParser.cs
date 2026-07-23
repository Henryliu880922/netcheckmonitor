using System.Globalization;
namespace NetCheckMonitor.Core.Report.Csv;

public sealed class CsvSessionParser
{
    public MonitoringSession Parse(string filePath)
    {
        throw new NotImplementedException();
    }

    public MonitoringSession Parse(Stream stream)
    {
        throw new NotImplementedException();
    }

    public MonitoringSession Parse(IEnumerable<string> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);

        var session = new MonitoringSession();
         foreach (string line in lines)
        {
            IReadOnlyList<string> fields = CsvLineParser.Parse(line);
            if (fields.Count < 6 || fields[0] == "Timestamp")
            {
                continue;
            }
            if (!DateTime.TryParse(
                    fields[0],
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind,
                    out DateTime timestamp))
            {
                continue;
            }
            if (string.Equals(fields[1], "MARKER", StringComparison.Ordinal))
            {
            if (string.Equals(fields[2], "STARTED", StringComparison.Ordinal))
            {
                session.Start = timestamp;
            }
            else if (string.Equals(fields[2], "STOPPED", StringComparison.Ordinal))
            {
                session.End = timestamp;
                session.Stopped = true;
            }
            else if (string.Equals(fields[2], "EVENT_NOTE", StringComparison.Ordinal))
            {
                session.EventNotes.Add(new EventNote
            {
                Time = timestamp,
                Text = fields[5]
            });
            }
            else if (string.Equals(fields[2], "COMPUTER", StringComparison.Ordinal))
            {
                ParseComputer(fields[5], session);
            }
                continue;
            }
            if (!string.Equals(fields[1], "CHECK", StringComparison.Ordinal))
            {
                continue;
            }
            long.TryParse(fields[3], out long latency);
            session.Records.Add(new MonitoringRecord
            {
                Time = timestamp,
                Online = string.Equals(
                    fields[2],
                    "ONLINE",
                    StringComparison.Ordinal),
                Status = fields[2],
                Latency = latency,
                Target = fields[4],
                Detail = fields[5]
            });
            if (session.Start == DateTime.MaxValue)
            {
                session.Start = timestamp;
            }
            if (timestamp > session.End)
            {
                session.End = timestamp;
            }
        }
        session.Records.Sort(
            (left, right) => left.Time.CompareTo(right.Time));
        return session;
    }
    private static void ParseComputer(
    string detail,
    MonitoringSession session)
{
    int openBracket = detail.IndexOf(
        " [",
        StringComparison.Ordinal);

    int closeBracket = openBracket >= 0
        ? detail.IndexOf(']', openBracket + 2)
        : -1;

    if (openBracket > 0)
    {
        session.MachineName =
            detail[..openBracket].Trim();
    }

    if (openBracket >= 0 && closeBracket > openBracket)
    {
        session.MachineId =
            detail[(openBracket + 2)..closeBracket].Trim();
    }
}
}