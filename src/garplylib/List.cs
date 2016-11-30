using System.Diagnostics;

namespace Garply
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct List
    {
        public readonly Value Head;
        public readonly int TailIndex;

        internal List(Value head, int tailIndex)
        {
            Head = head;
            TailIndex = tailIndex;
        }

        internal string DebuggerDisplay => $"Head.Raw:{Head.Raw}, TailIndex:{TailIndex}";
    }
}
