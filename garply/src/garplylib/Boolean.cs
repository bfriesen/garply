using System.IO;
using System.Text;

namespace garply
{
    public class Boolean : ITyped, IOperand
    {
        public Boolean(bool value)
        {
            Value = value;
        }

        public Type Type => Type.BooleanType;

        public bool Value { get; }

        public void Write(Stream stream)
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                writer.Write(Value);
            }
        }
    }
}
