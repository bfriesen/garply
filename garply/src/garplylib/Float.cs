using System.IO;

namespace garply
{
    public class Float : ITyped, IOperand
    {
        public Float(double value)
        {
            Value = value;
        }

        public Type Type => Type.FloatType;

        public double Value { get; }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Value);
        }
    }
}
