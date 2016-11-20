namespace garply
{
    public class List : IFirstClassType
    {
        private List()
        {
        }

        private List(IFirstClassType head, List tail)
        {
            Head = head;
            Tail = tail;
        }

        public static List Empty { get; } = new List();

        public List Add(IFirstClassType item)
        {
            return new List(item, this);
        }

        public Type Type => Type.ListType;

        public IFirstClassType Head { get; }

        public List Tail { get; }

        public bool IsEmpty => Head == null;
    }
}
