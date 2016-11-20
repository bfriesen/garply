using System.IO;

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

        public void Write(BinaryWriter writer)
        {
            writer.Write(Value);
        }
    }
}
