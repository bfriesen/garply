using System;
using System.IO;
using System.Text;

namespace garply
{
    public class Integer : ITyped, IOperand
    {
        public Integer(long value)
        {
            Value = value;
        }

        public Type Type => Type.IntegerType;

        public long Value { get; }

        public void Write(Stream stream)
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                writer.Write(Value);
            }
        }
    }
}
