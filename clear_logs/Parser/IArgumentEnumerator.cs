using System;

namespace ClearLogs.Parser
{
    internal interface IArgumentEnumerator : IDisposable
    {
        string GetRemainingFromNext();

        string Next { get; }
        bool IsLast { get; }

        bool MoveNext();

        bool MovePrevious();

        string Current { get; }
    }
}