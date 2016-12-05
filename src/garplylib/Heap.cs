using System.Collections.Generic;
using System.Threading;

namespace Garply
{
    public sealed class Heap
    {
        private readonly List<string> _strings = new List<string>();
        private readonly List<List> _lists = new List<List>();
        private readonly List<Tuple> _tuples = new List<Tuple>();

        private readonly List<int> _stringReferenceCounts = new List<int>();
        private readonly List<int> _listReferenceCounts = new List<int>();
        private readonly List<int> _tupleReferenceCounts = new List<int>();

        private readonly Queue<int> _availableStringIndexes = new Queue<int>();
        private readonly Queue<int> _availableListIndexes = new Queue<int>();
        private readonly Queue<int> _availableTupleIndexes = new Queue<int>();

        private static readonly ThreadLocal<Heap> _instance = new ThreadLocal<Heap>(() => new Heap());

        private Heap() { }

        public static Value AllocateString(string rawValue)
        {
            var instance = _instance.Value;
            int stringId;
            if (instance._availableStringIndexes.Count > 0)
            {
                stringId = instance._availableStringIndexes.Dequeue();
                instance._strings[stringId] = rawValue;
            }
            else
            {
                stringId = instance._strings.Count;
                instance._strings.Add(rawValue);
                instance._stringReferenceCounts.Add(0);
            }
            var value = new Value(Types.String, stringId);
            IncrementStringRefCount(value);
            return value;
        }

        public static Value AllocateTuple(int arity, IExecutionContext context)
        {
            var items = new Value[arity];
            for (int i = 0; i < arity; i++)
            {
                items[i] = context.Pop();
            }
            return AllocateTuple(items);
        }

        public static Value AllocateTuple(params Value[] items)
        {
            var instance = _instance.Value;
            var tuple = new Tuple(items);
            int tupleId;
            if (instance._availableTupleIndexes.Count > 0)
            {
                tupleId = instance._availableTupleIndexes.Dequeue();
                instance._tuples[tupleId] = tuple;
            }
            else
            {
                tupleId = instance._tuples.Count;
                instance._tuples.Add(tuple);
                instance._tupleReferenceCounts.Add(0);
            }
            for (int i = 0; i < items.Length; i++)
            {
                IncrementRefCount(items[i]);
            }
            var value = new Value(Types.Tuple, tupleId);
            IncrementTupleRefCount(value);
            return value;
        }

        public static Value AllocateList(Value head, Value tail)
        {
            var instance = _instance.Value;
            var tailId = (int)tail.Raw;
            var list = new List(head, tailId);
            int listId;
            if (instance._availableListIndexes.Count > 0)
            {
                listId = instance._availableListIndexes.Dequeue();
                instance._lists[listId] = list;
            }
            else
            {
                listId = instance._lists.Count;
                instance._lists.Add(list);
                instance._listReferenceCounts.Add(0);
            }
            IncrementRefCount(head);
            if (tailId != 0) instance._listReferenceCounts[tailId]++;
            var value = new Value(Types.List, listId);
            IncrementListRefCount(value);
            return value;
        }

        public static string GetString(int stringIndex)
        {
            return _instance.Value._strings[stringIndex];
        }

        public static Tuple GetTuple(int tupleIndex)
        {
            return _instance.Value._tuples[tupleIndex];
        }

        public static List GetList(int listIndex)
        {
            return _instance.Value._lists[listIndex];
        }

        public static int IndexOf(string value)
        {
            return _instance.Value._strings.IndexOf(value);
        }

        public static int IndexOf(Tuple value)
        {
            return _instance.Value._tuples.IndexOf(value);
        }

        public static int IndexOf(List value)
        {
            return _instance.Value._lists.IndexOf(value);
        }

        public static void IncrementRefCount(Value value)
        {
            switch (value.Type)
            {
                case Types.String:
                    IncrementStringRefCount(value);
                    break;
                case Types.Tuple:
                    IncrementTupleRefCount(value);
                    break;
                case Types.List:
                    IncrementListRefCount(value);
                    break;
            }
        }

        public static void IncrementStringRefCount(Value stringValue)
        {
            _instance.Value._stringReferenceCounts[(int)stringValue.Raw]++;
        }

        public static void IncrementTupleRefCount(Value tupleValue)
        {
            _instance.Value._tupleReferenceCounts[(int)tupleValue.Raw]++;
        }

        public static void IncrementListRefCount(Value listValue)
        {
            _instance.Value._listReferenceCounts[(int)listValue.Raw]++;
        }

        public static void DecrementRefCount(Value value)
        {
            switch (value.Type)
            {
                case Types.String:
                    DecrementStringRefCount(value);
                    break;
                case Types.Tuple:
                    DecrementTupleRefCount(value);
                    break;
                case Types.List:
                    DecrementListRefCount(value);
                    break;
            }
        }

        public static void DecrementStringRefCount(Value stringValue)
        {
            var instance = _instance.Value;
            var stringIndex = (int)stringValue.Raw;
            instance._stringReferenceCounts[stringIndex]--;
            if (instance._stringReferenceCounts[stringIndex] == 0)
            {
                instance._strings[stringIndex] = null;
                instance._availableStringIndexes.Enqueue(stringIndex);
            }
        }

        public static void DecrementTupleRefCount(Value tupleValue)
        {
            var instance = _instance.Value;
            var tupleIndex = (int)tupleValue.Raw;
            instance._tupleReferenceCounts[tupleIndex]--;
            if (instance._tupleReferenceCounts[tupleIndex] == 0)
            {
                var tuple = GetTuple(tupleIndex);
                for (int i = 0; i < tuple.Items.Count; i++)
                {
                    var item = tuple.Items[i];
                    DecrementRefCount(item);
                }

                if (instance._tupleReferenceCounts[tupleIndex] == 0)
                {
                    instance._tuples[tupleIndex] = default(Tuple);
                    instance._availableTupleIndexes.Enqueue(tupleIndex);
                }
            }
        }

        public static void DecrementListRefCount(Value listValue)
        {
            DecrementListRefCount((int)listValue.Raw);
        }

        private static void DecrementListRefCount(int listIndex)
        {
            if (listIndex == 0) return;
            var instance = _instance.Value;
            instance._listReferenceCounts[listIndex]--;
            if (instance._listReferenceCounts[listIndex] == 0)
            {
                var list = GetList(listIndex);
                DecrementRefCount(list.Head);
                DecrementListRefCount(list.TailIndex);

                if (instance._listReferenceCounts[listIndex] == 0)
                {
                    instance._lists[listIndex] = default(List);
                    instance._availableListIndexes.Enqueue(listIndex);
                }
            }
        }
    }
}
