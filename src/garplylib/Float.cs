using System.Diagnostics;
using System.IO;

namespace garply
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Float : IFirstClassType, IOperand
    {
        private Float(IType type)
        {
            Type = type;
        }

        public Float(double value)
        {
            Value = value;
            Type = Types.Float;
        }

        public static Float Empty { get; } = new Float(Types.Empty);
        public static Float Error { get; } = new Float(Types.Error);

        public IType Type { get; }

        public double Value { get; }

        public void Write(BinaryWriter writer, IMetadataDatabase metadataDatabase)
        {
            writer.Write(Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (ReferenceEquals(obj, null)) return false;
            var other = obj as Float;
            if (other == null) return false;
            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        private string DebuggerDisplay
        {
            get
            {
                if (Type.Equals(Types.Empty))
                {
                    return "empty<float>";
                }
                else if (Type.Equals(Types.Error))
                {
                    return "error<float>";
                }
                else
                {
                    return Value.ToString();
                }
            }
        }
    }
}
