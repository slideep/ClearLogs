using CommandLine;

namespace ClearLogs
{
    public class Options
    {
        [Option('d', "directory", Required = true, HelpText = "Set the directory where to clean up log files.")]
        public string Directory { get; set; }
    }
}