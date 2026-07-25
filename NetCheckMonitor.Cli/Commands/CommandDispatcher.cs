namespace NetCheckMonitor.Cli.Commands;

internal static class CommandDispatcher
{
    public static int Execute(string[] args)
    {
        if (args.Length == 0)
        {
            HelpCommand.ShowGeneralHelp();
            return 1;
        }

        if (args[0].Equals("help", StringComparison.OrdinalIgnoreCase) ||
        args[0].Equals("-help", StringComparison.OrdinalIgnoreCase) ||
        args[0].Equals("--help", StringComparison.OrdinalIgnoreCase))
        {
            HelpCommand.ShowGeneralHelp();
            return 0;
        }

        if (args[0].Equals("version", StringComparison.OrdinalIgnoreCase) ||
        args[0].Equals("-version", StringComparison.OrdinalIgnoreCase) ||
        args[0].Equals("--version", StringComparison.OrdinalIgnoreCase))
        {
            VersionCommand.Execute();
            return 0;
        }

        if (args[0].Equals("report", StringComparison.OrdinalIgnoreCase))
        {
            return ReportCommand.Execute(args[1..]);
        }

        HelpCommand.ShowGeneralHelp();
        return 1;
    }
}