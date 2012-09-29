using System;

namespace ClearLogs.Parser
{
    [Flags]
    internal enum ParserState : ushort
    {
        Success             = 0x01,
        Failure             = 0x02,
        MoveOnNextElement   = 0x04
    }
}