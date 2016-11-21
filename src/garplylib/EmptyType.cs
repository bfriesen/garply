using System.Diagnostics;

namespace garply
{
    [DebuggerDisplay("empty<type>")]
    public class EmptyType : EmptyBase, IType
    {
        public override IType Type => this;
        public IName Name { get; } = new EmptyName();
        public IType BaseType => this;
    }
}
