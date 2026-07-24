using NetCheckMonitor.Cli.Commands;

if (args.Length == 1 &&
    args[0].Equals("version", StringComparison.OrdinalIgnoreCase))
{
    VersionCommand.Execute();
    return;
}

if (args.Length >= 1 &&
    args[0].Equals("report", StringComparison.OrdinalIgnoreCase))
{
    ReportCommand.Execute(args[1..]);
    return;
}

Console.WriteLine("NetCheckMonitor CLI");
Console.WriteLine();
Console.WriteLine("Usage:");
Console.WriteLine("  netcheckmonitor version");
Console.WriteLine(
    "  netcheckmonitor report --input <monitor.csv> --output <report.csv>");