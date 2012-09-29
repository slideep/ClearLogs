using ClearLogs.Attributes;
using ClearLogs.Help;
using ClearLogs.Options;

namespace ClearLogs
{
    public class CommandLineOptions : CommandLineOptionsBase
    {
        [Option("d", "directory", Required = true, HelpText = "Denotes log directory.")]
        public string Directory { get; set; }

        [Option("v", "verbose", DefaultValue = true, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [Option("i", "interactive", DefaultValue = false, HelpText = "Interactive mode between clearing logs.")]
        public bool IsInteractive { get; set; }

        [HelpOption]
        public string Usage
        {
            get
            {
                var help = new HelpText
                    {
                        Heading = new HeadingInfo("ClearLogs", "0.1"),
                        AdditionalNewLineAfterOption = true,
                        AddDashesToOption = true
                    };

                help.AddPreOptionsLine("Usage: ClearLogs -d [log directory]");
                help.AddPostOptionsLine(@"Example: ClearLogs -d C:\temp\logs");
                help.AddOptions(this);

                return help;
            }
        }
    }
}