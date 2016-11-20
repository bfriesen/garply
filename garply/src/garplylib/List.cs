namespace garply
{
    public class List : ITyped
    {
        private List()
        {
        }

        private List(ITyped head, List tail)
        {
            Head = head;
            Tail = tail;
        }

        public static List Empty { get; } = new List();

        public List Add(ITyped item)
        {
            return new List(item, this);
        }

        public Type Type => Type.ListType;

        public ITyped Head { get; }

        public List Tail { get; }

        public bool IsEmpty => Head == null;
    }
}
