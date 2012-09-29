namespace ClearLogs.Attributes
{
    /// <summary>
    /// Models an option that can accept multiple values.
    /// Must be applied to a field compatible with an <see cref="System.Collections.Generic.IList&lt;T&gt;"/> interface
    /// of <see cref="System.String"/> instances.
    /// </summary>
    public sealed class OptionListAttribute : OptionAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionListAttribute"/> class.
        /// </summary>
        /// <param name="shortName">The short name of the option or null if not used.</param>
        /// <param name="longName">The long name of the option or null if not used.</param>
        public OptionListAttribute(string shortName, string longName)
            : base(shortName, longName)
        {
            Separator = ':';
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionListAttribute"/> class.
        /// </summary>
        /// <param name="shortName">The short name of the option or null if not used.</param>
        /// <param name="longName">The long name of the option or null if not used.</param>
        /// <param name="separator">Values separator character.</param>
        public OptionListAttribute(string shortName, string longName, char separator)
            : base(shortName, longName)
        {
            Separator = separator;
        }

        /// <summary>
        /// Gets or sets the values separator character.
        /// </summary>
        public char Separator { get; set; }
    }
}