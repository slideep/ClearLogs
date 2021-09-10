using CommandLine;
using System;
using System.Collections.Generic;
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
        [STAThread]
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(o =>
            {
                if (!Exists(o.Directory))
                {
                    return;
                }

                var logFiles = CheckLogDirectoryFilesExist(o.Directory);


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

                            Console.WriteLine(
                                "Clearing log file '{0}' ({1} lines of text).", logFileName, logLines.Count);

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
                        ? string.Format("Success! Cleared all log files at {0} successfully.", o.Directory)
                        : string.Format("All clear! There wasn't any log files with log lines at {0} to clear.", o.Directory));

                Console.WriteLine("Press [Enter] to continue.");
                Console.ReadLine();
            });            
        }

        private static IEnumerable<string> CheckLogDirectoryFilesExist(string path)
        {
            _ = path ?? throw new ArgumentNullException(nameof(path));

            var logFiles = 
                Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);
            if (!logFiles.Any())
            {
                Console.WriteLine(
                    "The specified log directory '{0}' didn't contain any log files.", path);

                return Enumerable.Empty<string>();
            }

            return logFiles;
        }

        private static bool Exists(string path)
        {
            _ = path ?? throw new ArgumentNullException(nameof(path));

            if (!Directory.Exists(path))
            {
                Console.WriteLine(
                    "The specified log directory '{0}' doesn't exist!", path);

                return false;
            }

            return true;
        }
    }
}