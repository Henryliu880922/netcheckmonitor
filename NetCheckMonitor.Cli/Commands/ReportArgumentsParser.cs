namespace NetCheckMonitor.Cli.Commands;

internal static class ReportArgumentsParser
{
    public static ReportArguments Parse(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);

        if (args.Length != 4 ||
            !args[0].Equals("--input", StringComparison.OrdinalIgnoreCase) ||
            !args[2].Equals("--output", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Invalid report arguments.");
        }

        return new ReportArguments
        {
            InputPath = args[1],
            OutputPath = args[3]
        };
    }
}