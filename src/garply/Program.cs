using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace garply
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

            var f = new Float(123.456);
            var b1 = new Boolean(true);
            var b2 = new Boolean(false);
            var s = new String("Hello, world!");
            var l = List.Empty.Add(f).Add(b1).Add(b2).Add(s);
            var t = new Tuple(new IFirstClassType[] { f, b1, b2, s, l });

            var metadataDatabase = new MetadataDatabase();
            Types.RegisterTo(metadataDatabase);
            metadataDatabase.RegisterString(new String("Hello, world!"));

            var expression = new ExpressionBuilder()
                .Add(Instructions.LoadType(metadataDatabase.GetTypeId(Types.String), metadataDatabase))
                .Add(Instructions.LoadType(metadataDatabase.GetTypeId(Types.Boolean), metadataDatabase))
                .Add(Instructions.TypeEquals())
                .Add(Instructions.Return())
                .SetArity(1)
                .SetConstants(0)
                .SetVariables(0)
                .Build();

            var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true)) expression.Write(writer, metadataDatabase);
            var binary = stream.ToArray();

            stream.Position = 0;
            var expression2 = Expression.Read(stream, metadataDatabase);
        }

        private class MetadataDatabase : IMetadataDatabase
        {
            private readonly Dictionary<String, Integer> _stringToId = new Dictionary<String, Integer>();
            private readonly Dictionary<Integer, String> _idToString = new Dictionary<Integer, String>();

            private readonly Dictionary<Integer, Type> _idToType = new Dictionary<Integer, Type>();
            private readonly Dictionary<Type, Integer> _typeToId = new Dictionary<Type, Integer>();

            public void RegisterString(String value)
            {
                var hashCode = new Integer(value.GetHashCode());
                _idToString[hashCode] = value;
                _stringToId[value] = hashCode;
            }

            public void RegisterType(Type value)
            {
                var hashCode = new Integer(value.GetHashCode());
                _idToType[hashCode] = value;
                _typeToId[value] = hashCode;
            }

            public Integer GetStringId(String value)
            {
                return _stringToId[value];
            }

            public String LoadString(Integer id)
            {
                return _idToString[id];
            }

            public Integer GetTypeId(Type value)
            {
                return _typeToId[value];
            }

            public Type LoadType(Integer id)
            {
                return _idToType[id];
            }
        }
    }
}
