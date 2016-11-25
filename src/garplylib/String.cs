using System.Diagnostics;
using System.IO;

namespace Garply
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class String : IOperand
    {
        public String(string value)
        {
            Debug.Assert(value != null);
            Value = value;
        }

        public static String Empty { get; } = new String("");

        public string Value { get; }

        public void Write(Opcode opcode, BinaryWriter writer, IMetadataDatabase metadataDatabase)
        {
            var id = metadataDatabase.GetStringId(this);
            id.Write(opcode, writer, metadataDatabase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (ReferenceEquals(obj, null)) return false;
            var other = obj as String;
            if (other == null) return false;
            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = 0;
                foreach (var c in Value)
                {
                    hashcode = (hashcode * 397) ^ c;
                }
                return hashcode;
            }
        }

        internal string DebuggerDisplay => Value.ToString();
    }
}
