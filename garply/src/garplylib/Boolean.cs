using System.IO;

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

        public void Write(BinaryWriter writer)
        {
            writer.Write(Value);
        }
    }
}
