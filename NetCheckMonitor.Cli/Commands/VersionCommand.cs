using System.Reflection;

namespace NetCheckMonitor.Cli.Commands;

internal static class VersionCommand
{
    public static void Execute()
    {
        string informationalVersion = Assembly
            .GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion
            ?? "Unknown";

        string version = informationalVersion.Split('+')[0];

        Console.WriteLine("NetCheckMonitor CLI");
        Console.WriteLine($"Version {version}");
    }
}
