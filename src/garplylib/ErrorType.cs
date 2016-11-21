using System.Diagnostics;

namespace garply
{
    [DebuggerDisplay("error<type>")]
    public class ErrorType : ErrorBase, IType
    {
        public override IType Type => this;
        public IName Name { get; } = new ErrorName();
        public IType BaseType => this;
    }
}
