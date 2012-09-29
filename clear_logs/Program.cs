using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClearLogs.Parser;

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
            var options = new CommandLineOptions();
            if (!CommandLineParser.Default.ParseArguments(args, options))
            {
                Console.WriteLine(options.Usage);
                Console.ReadLine();
                return;
            }

            if (CheckLogDirectoryExists(options)) 
                return;

            var logFiles = 
                new Func<CommandLineOptions, IEnumerable<string>>(CheckLogDirectoryFilesExist);
                
            var isAnyLogFileCleared = false;

            foreach (var logFileName in logFiles.Invoke(options))
            {
                try
                {
                    var logLines = File.ReadAllLines(logFileName).ToList();
                    if (logLines.Count <= 0)
                    {
                        continue;
                    }

                    if (options.Verbose)
                    {
                        Console.WriteLine(
                            "Clearing log file '{0}' ({1} lines of text).", logFileName, logLines.Count);
                    }

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
                    ? string.Format("Success! Cleared all log files at {0} successfully.", options.Directory)
                    : string.Format("All clear! There wasn't any log files with log lines at {0} to clear.", options.Directory));

            if (!options.IsInteractive)
            {
                return;
            }
      
            Console.WriteLine("Press [Enter] to continue.");
            Console.ReadLine();
        }

        private static IEnumerable<string> CheckLogDirectoryFilesExist(CommandLineOptions options)
        {
            if (options == null) 
                throw new ArgumentNullException("options");

            var logFiles = 
                Directory.GetFiles(options.Directory, "*.*", SearchOption.TopDirectoryOnly);
            if (!logFiles.Any())
            {
                Console.WriteLine(
                    "The specified log directory '{0}' didn't contain any log files.", options.Directory);

                return Enumerable.Empty<string>();
            }

            return logFiles;
        }

        private static bool CheckLogDirectoryExists(CommandLineOptions options)
        {
            if (options == null) 
                throw new ArgumentNullException("options");

            if (!Directory.Exists(options.Directory))
            {
                Console.WriteLine(
                    "The specified log directory '{0}' doesn't exist!", options.Directory);

                return true;
            }

            return false;
        }
    }
}