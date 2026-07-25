using System.Reflection;

namespace NetCheckMonitor.Cli.Commands;

internal static class VersionCommand
{
    public static void Execute()
    {
        Version? version = Assembly
            .GetExecutingAssembly()
            .GetName()
            .Version;

        Console.WriteLine("NetCheckMonitor CLI");
        Console.WriteLine($"Version {version}");
    }
}
