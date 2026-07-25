namespace NetCheckMonitor.Cli.Commands;

internal static class HelpCommand
{
    public static void ShowGeneralHelp()
    {
        Console.WriteLine("NetCheckMonitor CLI");
        Console.WriteLine();

        Console.WriteLine("Usage:");
        Console.WriteLine("  netcheckmonitor help");
        Console.WriteLine("  netcheckmonitor version");
        Console.WriteLine(
            "  netcheckmonitor report --input <monitor.csv> --output <report.csv>");
    }

    public static void ShowReportHelp()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine(
            "  netcheckmonitor report --input <monitor.csv> --output <report.csv>");
    }
}