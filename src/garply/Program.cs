using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Garply
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ////var instruction = default(Instruction);
            //var instruction = Instructions.Nop();

            //var stream = new MemoryStream();
            //instruction.Write(stream);
            //var data = stream.ToArray();

            //stream.Position = 0;
            //instruction = Instruction.Read(stream);

            //instruction = default(Instruction);
            //stream = new MemoryStream();
            //instruction.Write(stream);

            var f = new Value(new Float(123.456));
            var b1 = new Value(Boolean.Get(true));
            var b2 = new Value(Boolean.Get(false));
            var s = new Value(new String("Hello, world!"));
            var l = new Value(List.Empty.Add(f).Add(b1).Add(b2).Add(s));
            var t = new Value(new Tuple(new Value[] { f, b1, b2, s, l }));

            foreach (var item in l.AsList)
            {

            }

            // match x on
            // integer i : i > 0 -> ...
            // integer -> ...
            // (integer i | i > 0) -> ...
            // (integer) -> ...
            // (x) -> ...
            // 
            // (_) -> ...
            // () -> ...
            // [integer i : i > 0, integer j : j > 0] -> ...
            // _ -> ...

            var helloWorld = new String("Hello, world!");

            var metadataDatabase = new MetadataDatabase();
            metadataDatabase.RegisterString(helloWorld);

            var expression = new ExpressionBuilder()
                //.Add(Instructions.LoadFloat(new Float(123.45)))
                //.Add(Instructions.LoadInteger(new Integer(123)))
                //.Add(Instructions.LoadBoolean(Boolean.True))
                //.Add(Instructions.LoadString(metadataDatabase.GetStringId(helloWorld), metadataDatabase))
                //.Add(Instructions.LoadType(metadataDatabase.GetTypeId(Types.String), metadataDatabase))
                //.Add(Instructions.PushArg())
                //.Add(Instructions.LoadFloat(f))
                //.Add(Instructions.NewTuple(new Integer(2)))
                //.Add(Instructions.ListEmpty())
                //.Add(Instructions.LoadType(metadataDatabase.GetTypeId(Types.String), metadataDatabase))
                //.Add(Instructions.ListAdd())
                //.Add(Instructions.LoadFloat(new Float(123.45)))
                //.Add(Instructions.ListAdd())
                //.Add(Instructions.LoadInteger(new Integer(123)))
                //.Add(Instructions.ListAdd())
                .Add(Instructions.PushArg())
                .Add(Instructions.GetType())
                .Add(Instructions.LoadType(Types.Value))
                .Add(Instructions.TypeIs())
                .Add(Instructions.Return())
                .Build();

            var context = new ExecutionContext();
            context.Push(f);

            var x = expression.Evaluate(context);

            var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true)) expression.Write(writer, metadataDatabase);
            var binary = stream.ToArray();

            stream.Position = 0;
            var expression2 = Expression.Read(stream, metadataDatabase);
            context = new ExecutionContext();
            context.Push(f);
            var x2 = expression2.Evaluate(context);
        }

        private class ExecutionContext : IExecutionContext
        {
            private readonly Stack<Value> _stack = new Stack<Value>();
            public Value Pop() => _stack.Pop();
            public void Push(Value value) => _stack.Push(value);
        }

        private class MetadataDatabase : IMetadataDatabase
        {
            private readonly Dictionary<String, Integer> _stringToId = new Dictionary<String, Integer>();
            private readonly Dictionary<Integer, String> _idToString = new Dictionary<Integer, String>();

            private readonly Dictionary<Tuple, Integer> _tupleToId = new Dictionary<Tuple, Integer>();
            private readonly Dictionary<Integer, Tuple> _idToTuple = new Dictionary<Integer, Tuple>();

            private readonly Dictionary<List, Integer> _listToId = new Dictionary<List, Integer>();
            private readonly Dictionary<Integer, List> _idToList = new Dictionary<Integer, List>();

            public void RegisterString(String value)
            {
                var hashCode = new Integer(value.GetHashCode());
                _idToString[hashCode] = value;
                _stringToId[value] = hashCode;
            }

            public Integer GetStringId(String value)
            {
                return _stringToId[value];
            }

            public String LoadString(Integer id)
            {
                return _idToString[id];
            }

            public void RegisterTuple(Tuple value)
            {
                var hashCode = new Integer(value.GetHashCode());
                _idToTuple[hashCode] = value;
                _tupleToId[value] = hashCode;
            }

            public Integer GetTupleId(Tuple value)
            {
                return _tupleToId[value];
            }

            public Tuple LoadTuple(Integer id)
            {
                return _idToTuple[id];
            }

            public void RegisterList(List value)
            {
                var hashCode = new Integer(value.GetHashCode());
                _idToList[hashCode] = value;
                _listToId[value] = hashCode;
            }

            public Integer GetListId(List value)
            {
                return _listToId[value];
            }

            public List LoadList(Integer id)
            {
                return _idToList[id];
            }
        }
    }
}
