using System;

namespace garply
{
    public class Tuple : ITyped
    {
        private readonly ITyped[] _items;

        public Tuple(params ITyped[] items)
        {
#if UNSTABLE
            if (items == null) throw new ArgumentNullException("items");
#endif
            _items = items;
        }

        public Type Type => Type.TupleType;

        public int Arity => _items.Length;

        public ITyped this[int index] => _items[index];
    }
}
