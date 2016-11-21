using System.Diagnostics;
using System.IO;

namespace garply
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Integer : IFirstClassType, IOperand
    {
        private Integer(IType type)
        {
            Type = type;
        }

        public Integer(long value)
        {
            Value = value;
            Type = Types.Integer;
        }

        public static Integer Empty { get; } = new Integer(Types.Empty);
        public static Integer Error { get; } = new Integer(Types.Error);

        public IType Type { get; }

        public long Value { get; }

        public void Write(BinaryWriter writer, IMetadataDatabase metadataDatabase)
        {
            writer.Write(Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (ReferenceEquals(obj, null)) return false;
            var other = obj as Integer;
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
                    return "empty<integer>";
                }
                else if (Type.Equals(Types.Error))
                {
                    return "error<integer>";
                }
                else
                {
                    return Value.ToString();
                }
            }
        }
    }
}
