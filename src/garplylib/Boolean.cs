using System.Diagnostics;
using System.IO;

namespace garply
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Boolean : IFirstClassType, IOperand
    {
        private Boolean(IType type)
        {
            Type = type;
        }

        public Boolean(bool value)
        {
            Value = value;
            Type = Types.Boolean;
        }

        public static Boolean True { get; } = new Boolean(true);
        public static Boolean False { get; } = new Boolean(false);

        public static Boolean Empty { get; } = new Boolean(Types.Empty);
        public static Boolean Error { get; } = new Boolean(Types.Error);

        public IType Type { get; }

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

        internal string DebuggerDisplay
        {
            get
            {
                if (Type.Equals(Types.Empty))
                {
                    return "empty<boolean>";
                }
                else if (Type.Equals(Types.Error))
                {
                    return "error<boolean>";
                }
                else
                {
                    return Value.ToString();
                }
            }
        }
    }
}
