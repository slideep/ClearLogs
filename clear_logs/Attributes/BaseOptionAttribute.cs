using System;

namespace ClearLogs.Attributes
{
    /// <summary>
    ///     Provides base properties for creating an attribute, used to define rules for command line parsing.
    /// </summary>
    public abstract class BaseOptionAttribute : Attribute
    {
        private object _defaultValue;
        private string _shortName;

        /// <summary>
        ///     Short name of this command line option. You can use only one character.
        /// </summary>
        public string ShortName
        {
            get => _shortName;
            internal set
            {
                if (value != null && value.Length > 1)
                    throw new ArgumentException("shortName");

                _shortName = value;
            }
        }

        /// <summary>
        ///     Long name of this command line option. This name is usually a single english word.
        /// </summary>
        public string LongName { get; internal set; }

        /// <summary>
        ///     True if this command line option is required.
        /// </summary>
        public virtual bool Required { get; set; }

        /// <summary>
        ///     Gets or sets mapped property default value.
        /// </summary>
        public object DefaultValue
        {
            get => _defaultValue;
            set
            {
                _defaultValue = value;
                HasDefaultValue = true;
            }
        }

        internal bool HasShortName => !string.IsNullOrWhiteSpace(_shortName);

        internal bool HasLongName => !string.IsNullOrWhiteSpace(LongName);

        internal bool HasDefaultValue { get; private set; }

        /// <summary>
        ///     A short description of this command line option. Usually a sentence summary.
        /// </summary>
        public string HelpText { get; set; }
    }
}