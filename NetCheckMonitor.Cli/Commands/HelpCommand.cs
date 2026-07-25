namespace NetCheckMonitor.Cli.Commands;

internal static class HelpCommand
{
    public static void ShowGeneralHelp()
    {
        Console.WriteLine("NetCheckMonitor CLI");
        Console.WriteLine();

        Console.WriteLine("Usage:");
        Console.WriteLine("  netcheckmonitor <command> [options]");
        Console.WriteLine();

        Console.WriteLine("Commands:");
        Console.WriteLine("  help       Show help information.");
        Console.WriteLine("  version    Show application version.");
        Console.WriteLine("  report     Generate a daily availability report.");
        Console.WriteLine();

        Console.WriteLine("Examples:");
        Console.WriteLine("  netcheckmonitor help");
        Console.WriteLine("  netcheckmonitor --help");
        Console.WriteLine("  netcheckmonitor -help");
        Console.WriteLine();

        Console.WriteLine("  netcheckmonitor version");
        Console.WriteLine("  netcheckmonitor --version");
        Console.WriteLine("  netcheckmonitor -version");
        Console.WriteLine();

        Console.WriteLine("  netcheckmonitor report --input monitor.csv --output report.csv");
    }

    public static void ShowReportHelp()
    {
        Console.WriteLine("Report Command");
        Console.WriteLine();

        Console.WriteLine("Usage:");
        Console.WriteLine("  netcheckmonitor report --input <monitor.csv> --output <report.csv>");
        Console.WriteLine();

        Console.WriteLine("Options:");
        Console.WriteLine("  --input    Input monitor CSV file.");
        Console.WriteLine("  --output   Output report CSV file.");
        Console.WriteLine();

        Console.WriteLine("Example:");
        Console.WriteLine("  netcheckmonitor report --input monitor.csv --output report.csv");
    }
}