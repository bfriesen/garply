using System.Diagnostics;

namespace garply
{
    [DebuggerDisplay("empty<name>")]
    public class EmptyName : EmptyBase, IName
    {
        public string Value => "";
        public IName ParentName => this;
    }
}
