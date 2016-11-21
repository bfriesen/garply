using System.Collections;
using System.Diagnostics;

namespace garply
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class List : IFirstClassType, IEnumerable
    {
        private List(IType type)
        {
            Head = EmptyValue.Instance;
            Tail = this;
            Type = type;
            Count = new Integer(0);
        }

        private List(IFirstClassType head, List tail)
        {
            Head = head;
            Tail = tail;
            Type = Types.List;
            Count = new Integer(tail.Count.Value + 1);
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
        public Integer Count { get; }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ListEnumerator(this);
        }

        private class ListEnumerator : IEnumerator
        {
            private readonly List _seed;
            private List _list;

            public ListEnumerator(List list)
            {
                _seed = list.Add(ErrorValue.Instance);
                _list = _seed;
            }

            public object Current => _list.Head;

            public bool MoveNext()
            {
                _list = _list.Tail;
                return !_list.Type.Equals(Types.Empty);
            }

            public void Reset()
            {
                _list = _seed;
            }
        }

        internal string DebuggerDisplay
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
                    return $"list({Count.Value})";
                }
            }
        }
    }
}
