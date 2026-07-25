namespace NetCheckMonitor.Cli.Commands;

internal sealed class ReportArguments
{
    public required string InputPath { get; init; }

    public required string OutputPath { get; init; }
}