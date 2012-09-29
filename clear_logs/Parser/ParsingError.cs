using ClearLogs.Options;

namespace ClearLogs.Parser
{
    /// <summary>
    /// Models a parsing error.
    /// </summary>
    public class ParsingError
    {
        internal ParsingError()
        {
            BadOption = new BadOptionInfo();
        }

        internal ParsingError(string shortName, string longName, bool format)
        {
            BadOption = new BadOptionInfo(shortName, longName);
            ViolatesFormat = format;
        }
        
        /// <summary>
        /// Gets or a the bad parsed option.
        /// </summary>
        /// <value>
        /// The bad option.
        /// </value>
        public BadOptionInfo BadOption { get; private set; }

        
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ParsingError"/> violates required.
        /// </summary>
        /// <value>
        /// <c>true</c> if violates required; otherwise, <c>false</c>.
        /// </value>
        public bool ViolatesRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ParsingError"/> violates format.
        /// </summary>
        /// <value>
        /// <c>true</c> if violates format; otherwise, <c>false</c>.
        /// </value>
        public bool ViolatesFormat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ParsingError"/> violates mutual exclusiveness.
        /// </summary>
        /// <value>
        /// <c>true</c> if violates mutual exclusiveness; otherwise, <c>false</c>.
        /// </value>
        public bool ViolatesMutualExclusiveness { get; set; }
    }
}