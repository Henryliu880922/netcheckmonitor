using NetCheckMonitor.Cli.Commands;

if (args.Length == 1 &&

    args[0].Equals("help", StringComparison.OrdinalIgnoreCase))
{
    HelpCommand.ShowGeneralHelp();
    return;
}

if (args.Length == 1 &&
    args[0].Equals("version", StringComparison.OrdinalIgnoreCase))
{
    VersionCommand.Execute();
    return;
}

if (args.Length >= 1 &&
    args[0].Equals("report", StringComparison.OrdinalIgnoreCase))
{
    Environment.ExitCode = ReportCommand.Execute(args[1..]);
    return;
}

HelpCommand.ShowGeneralHelp();
Environment.ExitCode = CommandDispatcher.Execute(args); 