using System.IO;
using NetCheckMonitor.Cli;
using Xunit;

namespace NetCheckMonitor.Cli.Tests;

public class ReportRunnerTests
{
    [Fact]
    public void Run_CreatesExpectedReport()
    {
        string input = Path.GetTempFileName();
        string output = Path.GetTempFileName();

        try
        {
            File.WriteAllText(
                input,
                """
                Timestamp,Type,Status,Latency,Target,Detail
                2026-07-24T10:00:00+08:00,MARKER,STARTED,0,,Started
                2026-07-24T10:00:05+08:00,CHECK,ONLINE,15,1.1.1.1,OK
                2026-07-24T10:00:10+08:00,CHECK,OFFLINE,0,1.1.1.1,Timeout
                2026-07-24T10:00:20+08:00,CHECK,ONLINE,20,1.1.1.1,OK
                2026-07-24T10:00:25+08:00,MARKER,STOPPED,0,,Stopped
                """);

            var runner = new ReportRunner();

            runner.Run(input, output);

            string content = File.ReadAllText(output);

            Assert.Contains(
                "Day,MachineName,MachineId,Availability,CheckCount,OfflineCount,AverageLatency,MaximumLatency",
                content);

            Assert.Contains("2026-07-24", content);
            Assert.Contains(",3,1,17.5,20", content);
        }
        finally
        {
            File.Delete(input);
            File.Delete(output);
        }
    }
    [Fact]
    public void Run_InputFileDoesNotExist_ThrowsFileNotFoundException()
    {
        string input = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString() + ".csv");

        string output = Path.GetTempFileName();

        try
        {
            var runner = new ReportRunner();

            Assert.Throws<FileNotFoundException>(
                () => runner.Run(input, output));
        }
        finally
        {
            if (File.Exists(output))
            {
                File.Delete(output);
            }
        }
    }
}