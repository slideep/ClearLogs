namespace ClearLogs.Options
{
    /// <summary>
    /// Models a bad parsed option.
    /// </summary>
    public sealed class BadOptionInfo
    {
        internal BadOptionInfo()
        {
        }
        
        internal BadOptionInfo(string shortName, string longName)
        {
            ShortName = shortName;
            LongName = longName;
        }
        
        /// <summary>
        /// The short name of the option
        /// </summary>
        /// <value>Returns the short name of the option.</value>
        public string ShortName
        {
            get;
            internal set;
        }
        
        /// <summary>
        /// The long name of the option
        /// </summary>
        /// <value>Returns the long name of the option.</value>
        public string LongName {
            get;
            internal set;
        }
    }
}