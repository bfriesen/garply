using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Garply
{
    internal static  class Heap
    {
        private class HeapInstance
        {
            public readonly List<string> Strings = new List<string>();
            public readonly List<List> Lists = new List<List>() { new List(default(Value), 0) };
            public readonly List<Tuple> Tuples = new List<Tuple>() { new Tuple(new Value[0]) };
            public readonly List<Expression> Expressions = new List<Expression>();

            public readonly List<int> StringReferenceCounts = new List<int>();
            public readonly List<int> ListReferenceCounts = new List<int>() { int.MaxValue / 2 };
            public readonly List<int> TupleReferenceCounts = new List<int>() { int.MaxValue / 2 };
            public readonly List<int> ExpressionReferenceCounts = new List<int>();

            public readonly Queue<int> AvailableStringIndexes = new Queue<int>();
            public readonly Queue<int> AvailableListIndexes = new Queue<int>();
            public readonly Queue<int> AvailableTupleIndexes = new Queue<int>();
            public readonly Queue<int> AvailableExpressionIndexes = new Queue<int>();

            public readonly Dictionary<long, int> StringIndexLookup = new Dictionary<long, int>();
        }

        private static readonly ThreadLocal<HeapInstance> _instance = new ThreadLocal<HeapInstance>(() => new HeapInstance());

        public static Value AllocateString(string rawValue)
        {
            var instance = _instance.Value;
            var stringId = AllocateString(instance, rawValue);
            var value = new Value(Types.@string, stringId);
            return value;
        }

        public static Value AllocatePersistentString(string rawValue)
        {
            var instance = _instance.Value;
            var databaseId = rawValue.GetLongHashCode();

            int stringId;
            if (!instance.StringIndexLookup.TryGetValue(databaseId, out stringId))
            {
                stringId = AllocateString(instance, rawValue);
                instance.StringIndexLookup.Add(databaseId, stringId);
            }

            var value = new Value(Types.@string, stringId);
            return value;
        }

        private static int AllocateString(HeapInstance instance, string rawValue)
        {
            int stringId;
            if (instance.AvailableStringIndexes.Count > 0)
            {
                stringId = instance.AvailableStringIndexes.Dequeue();
                instance.Strings[stringId] = rawValue;
            }
            else
            {
                stringId = instance.Strings.Count;
                instance.Strings.Add(rawValue);
                instance.StringReferenceCounts.Add(0);
            }
            return stringId;
        }

        public static Value AllocateTuple(int arity, ExecutionContext context)
        {
            var items = new Value[arity];
            for (int i = arity - 1; i >= 0; i--)
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
            if (instance.AvailableTupleIndexes.Count > 0)
            {
                tupleId = instance.AvailableTupleIndexes.Dequeue();
                instance.Tuples[tupleId] = tuple;
            }
            else
            {
                tupleId = instance.Tuples.Count;
                instance.Tuples.Add(tuple);
                instance.TupleReferenceCounts.Add(0);
            }
            var value = new Value(Types.tuple, tupleId);
            return value;
        }

        public static Value AllocateList(IEnumerable<Value> items)
        {
            var list = Empty.List;
            foreach (var item in items.Reverse())
            {
                list = AllocateList(item, list);
            }
            return list;
        }

        public static Value AllocateList(Value head, Value tail)
        {
            var instance = _instance.Value;
            var tailId = (int)tail.Raw;
            var list = new List(head, tailId);
            int listId;
            if (instance.AvailableListIndexes.Count > 0)
            {
                listId = instance.AvailableListIndexes.Dequeue();
                instance.Lists[listId] = list;
            }
            else
            {
                listId = instance.Lists.Count;
                instance.Lists.Add(list);
                instance.ListReferenceCounts.Add(0);
            }
            var value = new Value(Types.list, listId);
            return value;
        }

        public static Value AllocateExpression(Types type, Instruction[] instructions)
        {
            var instance = _instance.Value;
            var expression = new Expression(type, instructions);
            int expressionId;
            if (instance.AvailableExpressionIndexes.Count > 0)
            {
                expressionId = instance.AvailableExpressionIndexes.Dequeue();
                instance.Expressions[expressionId] = expression;
            }
            else
            {
                expressionId = instance.Expressions.Count;
                instance.Expressions.Add(expression);
                instance.ExpressionReferenceCounts.Add(0);
            }
            var value = new Value(Types.expression, expressionId);
            return value;
        }

        public static string GetString(int stringIndex)
        {
            return _instance.Value.Strings[stringIndex];
        }

        public static Tuple GetTuple(int tupleIndex)
        {
            return _instance.Value.Tuples[tupleIndex];
        }

        public static List GetList(int listIndex)
        {
            return _instance.Value.Lists[listIndex];
        }

        public static Expression GetExpression(int expressionIndex)
        {
            return _instance.Value.Expressions[expressionIndex];
        }

        public static int IndexOf(string value)
        {
            return _instance.Value.Strings.IndexOf(value);
        }

        public static int IndexOf(Tuple value)
        {
            return _instance.Value.Tuples.IndexOf(value);
        }

        public static int IndexOf(List value)
        {
            return _instance.Value.Lists.IndexOf(value);
        }

        public static int IndexOf(Expression value)
        {
            return _instance.Value.Expressions.IndexOf(value);
        }

        public static void AddRef(this Value value)
        {
            switch (value.Type)
            {
                case Types.@string:
                    _instance.Value.StringReferenceCounts[(int)value.Raw]++;
                    break;
                case Types.tuple:
                    _instance.Value.TupleReferenceCounts[(int)value.Raw]++;
                    break;
                case Types.list:
                    _instance.Value.ListReferenceCounts[(int)value.Raw]++;
                    break;
                case Types.expression:
                    _instance.Value.ExpressionReferenceCounts[(int)value.Raw]++;
                    break;
            }
        }

        public static void RemoveRef(this Value value)
        {
            switch (value.Type)
            {
                case Types.@string:
                    DecrementStringRefCount(value);
                    break;
                case Types.tuple:
                    DecrementTupleRefCount(value);
                    break;
                case Types.list:
                    DecrementListRefCount(value);
                    break;
                case Types.expression:
                    DecrementExpressionRefCount(value);
                    break;
            }
        }

        private static void DecrementStringRefCount(Value stringValue)
        {
            var instance = _instance.Value;
            var stringIndex = (int)stringValue.Raw;
            instance.StringReferenceCounts[stringIndex]--;
            if (instance.StringReferenceCounts[stringIndex] == 0)
            {
                instance.StringIndexLookup.Remove(instance.Strings[stringIndex].GetLongHashCode());
                instance.Strings[stringIndex] = null;
                instance.AvailableStringIndexes.Enqueue(stringIndex);
            }
        }

        private static void DecrementTupleRefCount(Value tupleValue)
        {
            var instance = _instance.Value;
            var tupleIndex = (int)tupleValue.Raw;
            instance.TupleReferenceCounts[tupleIndex]--;
            if (instance.TupleReferenceCounts[tupleIndex] == 0)
            {
                var tuple = GetTuple(tupleIndex);
                for (int i = 0; i < tuple.Items.Count; i++)
                {
                    tuple.Items[i].RemoveRef();
                }

                if (instance.TupleReferenceCounts[tupleIndex] == 0)
                {
                    instance.Tuples[tupleIndex] = default(Tuple);
                    instance.AvailableTupleIndexes.Enqueue(tupleIndex);
                }
            }
        }

        private static void DecrementListRefCount(Value listValue)
        {
            DecrementListRefCount((int)listValue.Raw);
        }

        private static void DecrementListRefCount(int listIndex)
        {
            if (listIndex == 0) return;
            var instance = _instance.Value;
            instance.ListReferenceCounts[listIndex]--;
            if (instance.ListReferenceCounts[listIndex] == 0)
            {
                var list = GetList(listIndex);
                list.Head.RemoveRef();
                DecrementListRefCount(list.TailIndex);

                if (instance.ListReferenceCounts[listIndex] == 0)
                {
                    instance.Lists[listIndex] = default(List);
                    instance.AvailableListIndexes.Enqueue(listIndex);
                }
            }
        }

        private static void DecrementExpressionRefCount(Value expressionValue)
        {
            var instance = _instance.Value;
            var expressionIndex = (int)expressionValue.Raw;
            instance.ExpressionReferenceCounts[expressionIndex]--;
            if (instance.ExpressionReferenceCounts[expressionIndex] == 0)
            {
                instance.Expressions[expressionIndex] = default(Expression);
                instance.AvailableExpressionIndexes.Enqueue(expressionIndex);
            }
        }

        public static string Dump
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append("Strings[")
                    .Append(string.Join(", ",
                        _instance.Value.Strings.Select((s, i) => new { s, i })
                            .Zip(_instance.Value.StringReferenceCounts,
                            (x, c) => $"({c},{x.s ?? "|Empty|"},{_instance.Value.AvailableStringIndexes.Contains(x.i)})")))
                    .AppendLine("]");

                sb.Append("Tuples[")
                    .Append(string.Join(", ",
                        _instance.Value.Tuples.Select((t, i) => new { t, i })
                            .Zip(_instance.Value.TupleReferenceCounts,
                            (x, c) => $"({c},{(x.t.IsEmpty ? "|Empty|" : x.t.ToString())},{_instance.Value.AvailableTupleIndexes.Contains(x.i)})")))
                    .AppendLine("]");

                sb.Append("Lists[")
                    .Append(string.Join(", ",
                        _instance.Value.Lists.Select((l, i) => new { l, i })
                            .Zip(_instance.Value.ListReferenceCounts,
                            (x, c) => $"({c},{(x.l.IsEmpty ? "|Empty|" : x.l.ToString())},{_instance.Value.AvailableListIndexes.Contains(x.i)})")))
                    .AppendLine("]");

                sb.Append("Expressions[")
                    .Append(string.Join(", ",
                        _instance.Value.Expressions.Select((e, i) => new { e, i })
                            .Zip(_instance.Value.ExpressionReferenceCounts,
                            (x, c) => $"({c},{(x.e.IsEmpty ? "|Empty|" : x.e.ToString(true))},{_instance.Value.AvailableExpressionIndexes.Contains(x.i)})")))
                    .Append(']');

                return sb.ToString();
            }
        }
    }
}
