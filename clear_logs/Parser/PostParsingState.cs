using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ClearLogs.Parser
{
    /// <summary>
    /// Models a type that records the parser state afeter parsing.
    /// </summary>
    public sealed class PostParsingState
    {
        private static readonly object SyncObject = new object();

        public PostParsingState(IEnumerable<ParsingError> errors)
        {
            if (errors == null) 
                throw new ArgumentNullException("errors");

            ParsingErrors = new List<ParsingError>(errors);
        }

        /// <summary>
        /// Gets a list of parsing errors.
        /// </summary>
        /// <value>
        /// Parsing errors.
        /// </value>
        public ReadOnlyCollection<ParsingError> Errors { get { return new ReadOnlyCollection<ParsingError>(ParsingErrors);} }

        private List<ParsingError> ParsingErrors { get; set; } 

        public void AddError(ParsingError error)
        {
            if (error == null) 
                throw new ArgumentNullException("error");

            lock (SyncObject)
            {
                ParsingErrors.Add(error);
            }
        }
    }
}