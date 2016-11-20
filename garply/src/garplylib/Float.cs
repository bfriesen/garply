using System.IO;
using System.Text;

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

        public void Write(Stream stream)
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                writer.Write(Value);
            }
        }
    }
}
