using System.Diagnostics;

namespace garply
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class List : IFirstClassType
    {
        private List(IType type)
        {
            Head = new EmptyHead();
            Tail = this;
            Type = type;
        }

        private List(IFirstClassType head, List tail)
        {
            Head = head;
            Tail = tail;
            Type = Types.List;
        }

        public static List Empty { get; } = new List(Types.Empty);
        public static List Error { get; } = new List(Types.Error);

        public List Add(IFirstClassType item)
        {
            return new List(item, this);
        }

        public IType Type { get; }
        public IFirstClassType Head { get; }
        public List Tail { get; }

        private string DebuggerDisplay
        {
            get
            {
                if (Type.Equals(Types.Empty))
                {
                    return "empty<list>";
                }
                else if (Type.Equals(Types.Error))
                {
                    return "error<list>";
                }
                else
                {
                    return $"list({this.Count().Value})";
                }
            }
        }
    }
}
