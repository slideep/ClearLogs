using System;

namespace ClearLogs.Attributes
{
    /// <summary>
    /// Models an option specification.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property , AllowMultiple=false, Inherited=true)]
    public class OptionAttribute : BaseOptionAttribute
    {
        private string _mutuallyExclusiveSet;

        internal const string DefaultMutuallyExclusiveSet = "Default";

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionAttribute"/> class.
        /// </summary>
        /// <param name="shortName">The short name of the option or null if not used.</param>
        /// <param name="longName">The long name of the option or null if not used.</param>
        public OptionAttribute(string shortName, string longName)
        {
            if (!string.IsNullOrWhiteSpace(shortName))
                UniqueName = shortName;
            else if (!string.IsNullOrWhiteSpace(longName))
                UniqueName = longName;

            if (UniqueName == null)
                throw new InvalidOperationException();

            ShortName = shortName;
            LongName = longName;
        }

        internal string UniqueName { get; private set; }

        /// <summary>
        /// Gets or sets the option's mutually exclusive set.
        /// </summary>
        public string MutuallyExclusiveSet
        {
            get { return _mutuallyExclusiveSet; }
            set {
                _mutuallyExclusiveSet = string.IsNullOrWhiteSpace(value) ? DefaultMutuallyExclusiveSet : value;
            }
        }
    }
}