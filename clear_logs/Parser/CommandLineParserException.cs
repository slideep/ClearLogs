using System;
using System.Runtime.Serialization;

namespace ClearLogs.Parser
{
    /// <summary>
    /// This exception is thrown when a generic parsing error occurs.
    /// </summary>
    [Serializable]
    public sealed class CommandLineParserException : Exception
    {
        public CommandLineParserException()
        {
        }

        public CommandLineParserException(string message)
            : base(message)
        {
        }

        public CommandLineParserException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        private CommandLineParserException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}