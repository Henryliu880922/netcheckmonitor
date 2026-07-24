using NetCheckMonitor.Cli.Commands;

if (args.Length == 1 &&
    args[0].Equals("version", StringComparison.OrdinalIgnoreCase))
{
    VersionCommand.Execute();
    return;
}

Console.WriteLine("NetCheckMonitor CLI");
Console.WriteLine();
Console.WriteLine("Usage:");
Console.WriteLine("  netcheckmonitor version");
