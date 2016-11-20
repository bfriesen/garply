using System;

namespace garply
{
    public class Tuple : IFirstClassType
    {
        private readonly IFirstClassType[] _items;

        public Tuple(params IFirstClassType[] items)
        {
#if UNSTABLE
            if (items == null) throw new ArgumentNullException("items");
#endif
            _items = items;
        }

        public Type Type => Type.TupleType;

        public int Arity => _items.Length;

        public IFirstClassType this[int index] => _items[index];
    }
}
