using System;

namespace Garply
{
    public static class Empty
    {
        private static readonly Lazy<Value> _emptyList;
        private static readonly Lazy<Value> _emptyTuple;

        static Empty()
        {
            _emptyList = new Lazy<Value>(() =>
            {
                var list = Heap.AllocateList(default(Value), default(Value));
                Heap.IncrementListRefCount(list);
                return list;
            });

            _emptyTuple = new Lazy<Value>(() =>
            {
                var tuple = Heap.AllocateTuple();
                Heap.IncrementTupleRefCount(tuple);
                return tuple;
            });
        }

        public static Value List => _emptyList.Value;
        public static Value Tuple => _emptyTuple.Value;
    }
}
