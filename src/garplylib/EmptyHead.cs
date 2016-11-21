using System.Diagnostics;

namespace garply
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class EmptyHead : IFirstClassType
    {
        public IType Type => Types.Empty;

        private string DebuggerDisplay => "empty<head>";
    }
}
