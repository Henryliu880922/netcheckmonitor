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
    [Fact]
    public void Execute_Help_ReturnsExitCode0()
    {
        TextWriter originalOutput = Console.Out;
        var writer = new StringWriter();

        try
        {
            Console.SetOut(writer);

            int exitCode = CommandDispatcher.Execute(["help"]);

            Assert.Equal(0, exitCode);

            string output = writer.ToString();

            Assert.Contains("NetCheckMonitor CLI", output);
            Assert.Contains("Usage:", output);
        }
        finally
        {
            Console.SetOut(originalOutput);
        }
    }
    [Fact]
    public void Execute_UnknownCommand_ReturnsExitCode1()
    {
        TextWriter originalOutput = Console.Out;
        var writer = new StringWriter();

        try
        {
            Console.SetOut(writer);

            int exitCode = CommandDispatcher.Execute(["unknown"]);

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