using System.Diagnostics;
using System.Runtime.Versioning;

namespace NetCheckMonitor.Core.Services.Mac;

[SupportedOSPlatform("macos")]
internal static class MacCommandRunner
{
    public static string Run(string fileName, params string[] arguments)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            foreach (var argument in arguments)
            {
                startInfo.ArgumentList.Add(argument);
            }

            using var process = Process.Start(startInfo);

            if (process is null)
            {
                return string.Empty;
            }

            var output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            return process.ExitCode == 0
                ? output.Trim()
                : string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}