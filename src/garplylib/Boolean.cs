using System.Diagnostics;
using System.IO;

namespace Garply
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Boolean : IOperand
    {
        private Boolean(bool value)
        {
            Value = value;
        }

        public static Boolean True { get; } = new Boolean(true);
        public static Boolean False { get; } = new Boolean(false);

        public static Boolean Get(bool value)
        {
            return value ? True : False;
        }

        public bool Value { get; }

        public void Write(Opcode opcode, BinaryWriter writer, IMetadataDatabase metadataDatabase)
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

        internal string DebuggerDisplay => Value.ToString();
    }
}
