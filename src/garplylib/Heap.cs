using System.Collections.Generic;
using System.Threading;

namespace Garply
{
    public static class Heap
    {
        #region Pass-through

        public static void RegisterString(long id, string value)
        {
            HeapInstance.Instance.RegisterString(id, value);
        }

        public static Value AllocateString(string value)
        {
            return HeapInstance.Instance.AllocateString(value);
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
            return HeapInstance.Instance.AllocateTuple(items);
        }

        public static Value AllocateList(Value head, Value tail)
        {
            return HeapInstance.Instance.AllocateList(head, tail);
        }

        public static string GetString(int stringIndex)
        {
            return HeapInstance.Instance.GetString(stringIndex);
        }

        public static Tuple GetTuple(int tupleIndex)
        {
            return HeapInstance.Instance.GetTuple(tupleIndex);
        }

        public static List GetList(int listIndex)
        {
            return HeapInstance.Instance.GetList(listIndex);
        }

        public static int IndexOf(string value)
        {
            return HeapInstance.Instance.IndexOf(value);
        }

        public static int IndexOf(Tuple value)
        {
            return HeapInstance.Instance.IndexOf(value);
        }

        public static int IndexOf(List value)
        {
            return HeapInstance.Instance.IndexOf(value);
        }

        public static void IncrementRefCount(Value value)
        {
            HeapInstance.Instance.IncrementRefCount(value);
        }

        public static void IncrementStringRefCount(Value stringValue)
        {
            HeapInstance.Instance.IncrementStringRefCount(stringValue);
        }

        public static void IncrementTupleRefCount(Value tupleValue)
        {
            HeapInstance.Instance.IncrementTupleRefCount(tupleValue);
        }

        public static void IncrementListRefCount(Value listValue)
        {
            HeapInstance.Instance.IncrementListRefCount(listValue);
        }

        public static void DecrementRefCount(Value value)
        {
            HeapInstance.Instance.DecrementRefCount(value);
        }

        public static void DecrementStringRefCount(Value stringValue)
        {
            HeapInstance.Instance.DecrementStringRefCount(stringValue);
        }

        public static void DecrementTupleRefCount(Value tupleValue)
        {
            HeapInstance.Instance.DecrementTupleRefCount(tupleValue);
        }

        public static void DecrementListRefCount(Value listValue)
        {
            HeapInstance.Instance.DecrementListRefCount(listValue);
        }

        #endregion

        private class HeapInstance
        {
            public readonly Dictionary<long, int> _stringIdToIndexMap = new Dictionary<long, int>();

            public readonly List<string> _strings = new List<string>();
            public readonly List<List> _lists = new List<List>();
            public readonly List<Tuple> _tuples = new List<Tuple>();

            public readonly List<int> _stringReferenceCounts = new List<int>();
            public readonly List<int> _listReferenceCounts = new List<int>();
            public readonly List<int> _tupleReferenceCounts = new List<int>();

            public readonly Queue<int> _availableStringIndexes = new Queue<int>();
            public readonly Queue<int> _availableListIndexes = new Queue<int>();
            public readonly Queue<int> _availableTupleIndexes = new Queue<int>();

            private HeapInstance()
            {
            }

            private static readonly ThreadLocal<HeapInstance> _instance = new ThreadLocal<Heap.HeapInstance>(() => new HeapInstance());
            public static HeapInstance Instance => _instance.Value;

            public void RegisterString(long id, string value)
            {
                var stringValue = AllocateString(value);
                IncrementStringRefCount(stringValue);
                _stringIdToIndexMap.Add(id, (int)stringValue.Raw);
            }

            public Value AllocateString(string rawValue)
            {
                int stringId;
                if (_availableStringIndexes.Count > 0)
                {
                    stringId = _availableStringIndexes.Dequeue();
                    _strings[stringId] = rawValue;
                }
                else
                {
                    stringId = _strings.Count;
                    _strings.Add(rawValue);
                    _stringReferenceCounts.Add(0);
                }
                var value = new Value(Types.String, stringId);
                return value;
            }

            public Value AllocateTuple(params Value[] items)
            {
                var tuple = new Tuple(items);
                int tupleId;
                if (_availableTupleIndexes.Count > 0)
                {
                    tupleId = _availableTupleIndexes.Dequeue();
                    _tuples[tupleId] = tuple;
                }
                else
                {
                    tupleId = _tuples.Count;
                    _tuples.Add(tuple);
                    _tupleReferenceCounts.Add(0);
                }
                for (int i = 0; i < items.Length; i++)
                {
                    IncrementRefCount(items[i]);
                }
                var value = new Value(Types.Tuple, tupleId);
                return value;
            }

            public Value AllocateList(Value head, Value tail)
            {
                var tailId = (int)tail.Raw;
                var list = new List(head, tailId);
                int listId;
                if (_availableListIndexes.Count > 0)
                {
                    listId = _availableListIndexes.Dequeue();
                    _lists[listId] = list;
                }
                else
                {
                    listId = _lists.Count;
                    _lists.Add(list);
                    _listReferenceCounts.Add(0);
                }
                IncrementRefCount(head);
                if (tailId != 0) _listReferenceCounts[tailId]++;
                var value = new Value(Types.List, listId);
                return value;
            }

            public string GetString(int stringIndex)
            {
                return _strings[stringIndex];
            }

            public Tuple GetTuple(int tupleIndex)
            {
                return _tuples[tupleIndex];
            }

            public List GetList(int listIndex)
            {
                return _lists[listIndex];
            }

            public int IndexOf(string value)
            {
                return _strings.IndexOf(value);
            }

            public int IndexOf(Tuple value)
            {
                return _tuples.IndexOf(value);
            }

            public int IndexOf(List value)
            {
                return _lists.IndexOf(value);
            }

            public void IncrementRefCount(Value value)
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

            public void IncrementStringRefCount(Value stringValue)
            {
                _stringReferenceCounts[(int)stringValue.Raw]++;
            }

            public void IncrementTupleRefCount(Value tupleValue)
            {
                _tupleReferenceCounts[(int)tupleValue.Raw]++;
            }

            public void IncrementListRefCount(Value listValue)
            {
                _listReferenceCounts[(int)listValue.Raw]++;
            }

            public void DecrementRefCount(Value value)
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

            public void DecrementStringRefCount(Value stringValue)
            {
                var stringIndex = (int)stringValue.Raw;
                _stringReferenceCounts[stringIndex]--;
                if (_stringReferenceCounts[stringIndex] == 0)
                {
                    _strings[stringIndex] = null;
                    _availableStringIndexes.Enqueue(stringIndex);
                }
            }

            public void DecrementTupleRefCount(Value tupleValue)
            {
                var tupleIndex = (int)tupleValue.Raw;
                _tupleReferenceCounts[tupleIndex]--;
                if (_tupleReferenceCounts[tupleIndex] == 0)
                {
                    var tuple = GetTuple(tupleIndex);
                    for (int i = 0; i < tuple.Items.Count; i++)
                    {
                        var item = tuple.Items[i];
                        DecrementRefCount(item);
                    }

                    if (_tupleReferenceCounts[tupleIndex] == 0)
                    {
                        _tuples[tupleIndex] = default(Tuple);
                        _availableTupleIndexes.Enqueue(tupleIndex);
                    }
                }
            }

            public void DecrementListRefCount(Value listValue)
            {
                DecrementListRefCount((int)listValue.Raw);
            }

            private void DecrementListRefCount(int listIndex)
            {
                if (listIndex == 0) return;
                _listReferenceCounts[listIndex]--;
                if (_listReferenceCounts[listIndex] == 0)
                {
                    var list = GetList(listIndex);
                    DecrementRefCount(list.Head);
                    DecrementListRefCount(list.TailIndex);

                    if (_listReferenceCounts[listIndex] == 0)
                    {
                        _lists[listIndex] = default(List);
                        _availableListIndexes.Enqueue(listIndex);
                    }
                }
            }
        }
    }
}
