using System.Diagnostics;

namespace garply
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class EmptyValue : IFirstClassType
    {
        private EmptyValue()
        {
        }

        public static EmptyValue Instance { get; } = new EmptyValue();

        public IType Type => Types.Empty;

        internal string DebuggerDisplay => "empty";
    }
}
