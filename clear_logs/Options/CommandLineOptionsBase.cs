using System.Linq;
using ClearLogs.Parser;

namespace ClearLogs.Options
{
    /// <summary>
    ///     Provides the abstract base class for a strongly typed options target. Used when you need to get parsing errors.
    /// </summary>
    public abstract class CommandLineOptionsBase
    {
        protected CommandLineOptionsBase()
        {
            LastPostParsingState = new PostParsingState(Enumerable.Empty<ParsingError>().ToList().AsReadOnly());
        }

        private PostParsingState LastPostParsingState { get; }

        internal PostParsingState InternalLastPostParsingState => LastPostParsingState;
    }
}