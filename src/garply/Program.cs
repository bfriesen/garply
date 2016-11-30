using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Garply
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var metadatabase = new MetadataDatabase();
            var stringId = metadatabase.AddString("Hello, world!");

            var expression = new ExpressionBuilder()
                .Add(Instructions.PushArg())
                .Add(Instructions.GetType())
                .Add(Instructions.LoadType(Types.List))
                .Add(Instructions.TypeEquals())
                .Add(Instructions.Return())
                .Build();

            var context = new ExecutionContext();
            var list = Heap.AllocateList(new Value(3), Heap.AllocateList(new Value(2), Heap.AllocateList(new Value(1), Empty.List)));
            Heap.IncrementListRefCount(list);
            context.Push(list);

            var x = expression.Evaluate(context);
            Heap.DecrementRefCount(x);

            var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true)) expression.Write(writer, metadatabase);
            var binary = stream.ToArray();

            stream.Position = 0;
            var expression2 = Expression.Read(stream, metadatabase);
            context = new ExecutionContext();
            context.Push(Heap.AllocateList(new Value(3), Heap.AllocateList(new Value(2), Heap.AllocateList(new Value(1), Empty.List))));
            var x2 = expression2.Evaluate(context);
            Heap.DecrementRefCount(x);
        }

        private class ExecutionContext : IExecutionContext
        {
            private readonly Stack<Value> _stack = new Stack<Value>();
            public Value Pop() => _stack.Pop();
            public void Push(Value value) => _stack.Push(value);
        }

        private class MetadataDatabase : IMetadataDatabase
        {
            private static readonly ThreadLocal<Dictionary<long, Value>> __stringValues = new ThreadLocal<Dictionary<long, Value>>(() => new Dictionary<long, Value>());
            private static Dictionary<long, Value> _stringValues => __stringValues.Value;

            private readonly Dictionary<long, string> _rawStringValues = new Dictionary<long, string>();
            private readonly Dictionary<string, long> _stringIds = new Dictionary<string, long>();

            public long AddString(string rawValue)
            {
                var id = rawValue.GetLongHashCode();
                _rawStringValues.Add(id, rawValue);
                _stringIds.Add(rawValue, id);
                return id;
            }

            public Value LoadString(long id)
            {
                if (_stringValues.ContainsKey(id)) return _stringValues[id];
                var value = Heap.AllocateString(_rawStringValues[id]);
                Heap.IncrementStringRefCount(value);
                _stringValues.Add(id, value);
                return value;
            }

            public long GetStringId(string value)
            {
                return _stringIds[value];
            }
        }
    }
}
