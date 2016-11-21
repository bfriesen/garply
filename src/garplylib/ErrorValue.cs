using System.Diagnostics;

namespace garply
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class ErrorValue : IFirstClassType
    {
        private ErrorValue()
        {
        }

        public static ErrorValue Instance { get; } = new ErrorValue();

        public IType Type => Types.Empty;

        internal string DebuggerDisplay => "error";
    }
}
