using System;
using System.Diagnostics;

namespace garply
{
    [DebuggerDisplay("error<type>")]
    public class ErrorType : ErrorBase, IType
    {
        public override IType Type => this;
        public IName Name { get; } = new ErrorName();
        public IType BaseType => this;

        public Boolean Is(IType other)
        {
#if UNSTABLE
            if (other == null) throw new ArgumentNullException("other");
#endif
            if (Equals(other)) return Boolean.True;
            if (BaseType.Equals(Types.Empty)) return Boolean.False;
            return BaseType.Is(other);
        }
    }
}
