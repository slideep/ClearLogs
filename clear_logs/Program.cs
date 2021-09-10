using CommandLine;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace ClearLogs
{
    /// <summary>
    /// Clears all lines of text from the designated log file directory's files.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entrypoint for the console application.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(o =>
            {
                if (!Exists(o.Directory)) return;

                var logFiles = CheckLogDirectoryFilesExist(o.Directory).ToImmutableList();

                var isAnyLogFileCleared = false;

                foreach (var logFileName in logFiles)
                {
                    try
                    {
                        var logLines = File.ReadAllLines(logFileName).ToList();
                        if (logLines.Count <= 0)
                        {
                            continue;
                        }

                        Console.WriteLine($"Clearing log file '{logFileName}' ({logLines.Count:N} lines of text).");

                        File.WriteAllLines(logFileName, Enumerable.Empty<string>().ToArray());

                        isAnyLogFileCleared = true;
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        Console.WriteLine(e.Message);
                        return;
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(e.Message);
                        return;
                    }
                }

                Console.WriteLine(
                    isAnyLogFileCleared
                        ? $"Success! Cleared all log files at {o.Directory} successfully."
                        : $"All clear! There wasn't any log files with log lines at {o.Directory} to clear.");

                Console.WriteLine("Press [Enter] to continue.");
                Console.ReadLine();
            });
        }

        private static IEnumerable<string> CheckLogDirectoryFilesExist(string path)
        {
            _ = path ?? throw new ArgumentNullException(nameof(path));

            var logFiles =
                Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);
            if (logFiles.Any())
            {
                return logFiles;
            }

            Console.WriteLine(
                "The specified log directory '{0}' didn't contain any log files.", path);

            return Enumerable.Empty<string>();
        }

        private static bool Exists(string path)
        {
            _ = path ?? throw new ArgumentNullException(nameof(path));

            if (Directory.Exists(path))
            {
                return true;
            }

            Console.WriteLine(
                "The specified log directory '{0}' doesn't exist!", path);

            return false;
        }
    }
}