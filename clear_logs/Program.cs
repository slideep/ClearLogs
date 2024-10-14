using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ClearLogs;

public sealed class ClearLogsCommand : Command<ClearLogsCommand.Settings>
{
    private static readonly int MaxDegreeOfParallelism = Math.Min(8, Environment.ProcessorCount);

    public override int Execute(CommandContext context, Settings settings)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(settings);

        var directory = settings.Directory;

        if (!Exists(directory))
        {
            return 1; // Non-zero exit code for failure
        }

        var logFiles = GetLogFiles(directory);

        if (logFiles.Length == 0)
        {
            AnsiConsole.MarkupLine($"[yellow]The specified log directory '{directory}' contains no log files to clear.[/]");
            return 0;
        }

        var clearedCount = 0;
        var exceptions = new ConcurrentBag<Exception>();

        var stopwatch = Stopwatch.StartNew();

        Parallel.ForEach(
            Partitioner.Create(0, logFiles.Length),
            new ParallelOptions { MaxDegreeOfParallelism = MaxDegreeOfParallelism },
            (range, _) =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    try
                    {
                        if (ClearLogFile(logFiles[i]))
                        {
                            Interlocked.Increment(ref clearedCount);
                        }
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
            });

        stopwatch.Stop();

        foreach (var exception in exceptions)
        {
            AnsiConsole.WriteException(exception);
        }

        AnsiConsole.MarkupLine(clearedCount > 0
            ? $"[green]Success! Cleared {clearedCount} log files in '{directory}' successfully in {stopwatch.ElapsedMilliseconds} ms.[/]"
            : $"[yellow]No log files in '{directory}' had lines to clear.[/]");

        AnsiConsole.MarkupLine("[blue]Press Enter to exit.[/]");
        Console.ReadLine();

        return 0;
    }

    private static string[] GetLogFiles(string path) =>
        Directory.Exists(path)
            ? Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly)
            : Array.Empty<string>();

    private static bool ClearLogFile(string logFileName)
    {
        var fileInfo = new FileInfo(logFileName);

        if (fileInfo.Length == 0)
        {
            return false;
        }

        AnsiConsole.MarkupLine($"[cyan]Clearing log file '{logFileName}' ({fileInfo.Length:N0} bytes).[/]");

        using var fileStream = new FileStream(logFileName, FileMode.Truncate, FileAccess.Write, FileShare.None, 4096, FileOptions.WriteThrough);
        fileStream.SetLength(0);

        return true;
    }

    private static bool Exists(string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            AnsiConsole.MarkupLine("[red]The specified directory is invalid![/]");
            return false;
        }

        if (Directory.Exists(path))
        {
            return true;
        }

        AnsiConsole.MarkupLine($"[red]The specified log directory '{path}' doesn't exist![/]");
        return false;
    }

    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<Directory>")]
        [Description("The directory containing the log files to clear.")]
        public string Directory { get; set; } = string.Empty;
    }
}

public static class Program
{
    public static int Main(string[] args)
    {
        var app = new CommandApp<ClearLogsCommand>();
        return app.Run(args);
    }
}
