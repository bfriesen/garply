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

        public void Write(BinaryWriter writer, IMetadataDatabase metadataDatabase)
        {
            writer.Write(Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (ReferenceEquals(obj, null)) return false;
            var other = obj as Boolean;
            if (other == null) return false;
            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
