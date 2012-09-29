using System;

namespace ClearLogs.Attributes
{
    /// <summary>
    /// Models an option specification.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property , AllowMultiple=false, Inherited=true)]
    public class OptionAttribute : BaseOptionAttribute
    {
        private readonly string _uniqueName;
        private string _mutuallyExclusiveSet;

        internal const string DefaultMutuallyExclusiveSet = "Default";

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionAttribute"/> class.
        /// </summary>
        /// <param name="shortName">The short name of the option or null if not used.</param>
        /// <param name="longName">The long name of the option or null if not used.</param>
        public OptionAttribute(string shortName, string longName)
        {
            if (!string.IsNullOrEmpty(shortName))
                _uniqueName = shortName;
            else if (!string.IsNullOrEmpty(longName))
                _uniqueName = longName;

            if (_uniqueName == null)
                throw new InvalidOperationException();

            ShortName = shortName;
            LongName = longName;
        }

        internal string UniqueName
        {
            get { return _uniqueName; }
        }

        /// <summary>
        /// Gets or sets the option's mutually exclusive set.
        /// </summary>
        public string MutuallyExclusiveSet
        {
            get { return _mutuallyExclusiveSet; }
            set {
                _mutuallyExclusiveSet = string.IsNullOrEmpty(value) ? DefaultMutuallyExclusiveSet : value;
            }
        }
    }
}