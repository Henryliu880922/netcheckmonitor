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
        DateTime? pauseStart = null;
        DateTime? outageStart = null;
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
                else if (string.Equals(fields[2], "NETWORK", StringComparison.Ordinal))
                {
                    var network = ParseNetwork(fields[5]);
                    session.Networks.Add(network);
                }
                else if (string.Equals(fields[2], "PAUSED", StringComparison.Ordinal))
                {
                    if (!pauseStart.HasValue)
                    {
                        pauseStart = timestamp;
                    }
                }
                else if (string.Equals(fields[2], "RESUMED", StringComparison.Ordinal))
                {
                    if (pauseStart.HasValue)
                    {
                        session.PausePeriods.Add(new PausePeriod
                        {
                            Start = pauseStart.Value,
                            End = timestamp
                        });

                        pauseStart = null;
                    }
                    continue;
                }
                continue;
            }
            if (!string.Equals(fields[1], "CHECK", StringComparison.Ordinal))
            {
                continue;
            }

            if (string.Equals(fields[2], "OFFLINE", StringComparison.Ordinal))
            {
                if (!outageStart.HasValue)
                {
                    outageStart = timestamp;
                }
            }
            else if (string.Equals(fields[2], "ONLINE", StringComparison.Ordinal))
            {
                if (outageStart.HasValue)
                {
                    session.Outages.Add(new OutagePeriod
                    {
                        Start = outageStart.Value,
                        End = timestamp
                    });

                    outageStart = null;
                }
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
    private static NetworkInfo ParseNetwork(string detail)
    {
        var network = new NetworkInfo();

        foreach (var part in detail.Split(
                     ';',
                     StringSplitOptions.RemoveEmptyEntries))
        {
            var kv = part.Split('=', 2);

            if (kv.Length != 2)
            {
                continue;
            }

            switch (kv[0])
            {
                case "Adapter":
                    network.Adapter = kv[1];
                    break;

                case "Description":
                    network.Description = kv[1];
                    break;

                case "Type":
                    network.Type = kv[1];
                    break;
                case "SSID":
                    network.SSID = kv[1];
                    break;
                case "BSSID":
                    network.BSSID = kv[1];
                    break;
                case "Signal":
                    if (int.TryParse(kv[1], out int Signal))
                    {
                        network.Signal = Signal;
                    }
                    break;
                case "IPv4":
                    network.IPv4 = kv[1];
                    break;
                case "IPv6":
                    network.IPv6 = kv[1];
                    break;

                case "Gateway":
                    network.Gateway = kv[1];
                    break;

                case "DNS":
                    network.Dns = kv[1];
                    break;
                case "MAC":
                case "MacAddress":
                    network.MacAddress = kv[1];
                    break;
            }
        }
        return network;
    }
}