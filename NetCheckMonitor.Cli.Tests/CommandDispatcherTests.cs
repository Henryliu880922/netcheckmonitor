using System;
using System.IO;
using NetCheckMonitor.Cli.Commands;
using Xunit;

public class CommandDispatcherTests
{
    [Fact]
    public void Execute_NoArguments_ReturnsExitCode1()
    {
        TextWriter originalOutput = Console.Out;
        var writer = new StringWriter();

        try
        {
            Console.SetOut(writer);

            int exitCode = CommandDispatcher.Execute(Array.Empty<string>());

            Assert.Equal(1, exitCode);

            string output = writer.ToString();

            Assert.Contains("NetCheckMonitor CLI", output);
            Assert.Contains("Usage:", output);
        }
        finally
        {
            Console.SetOut(originalOutput);
        }
    }
}