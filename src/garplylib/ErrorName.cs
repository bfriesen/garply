using System.Diagnostics;

namespace garply
{
    [DebuggerDisplay("error<name>")]
    public class ErrorName : ErrorBase, IName
    {
        public string Value => "";
        public IName ParentName => this;
    }
}
