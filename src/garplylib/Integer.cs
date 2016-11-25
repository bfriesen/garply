using System.Diagnostics;
using System.IO;

namespace Garply
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Integer : IOperand
    {
        public Integer(long value)
        {
            Value = value;
        }

        public long Value { get; }

        public void Write(Opcode opcode, BinaryWriter writer, IMetadataDatabase metadataDatabase)
        {
            switch (opcode)
            {
                case Opcode.NewTuple:
                case Opcode.TupleItem:
                    writer.Write((byte)Value);
                    break;
                case Opcode.LoadType:
                //case Opcode.LoadList:
                case Opcode.LoadString:
                    writer.Write((uint)Value);
                    break;
                default:
                    writer.Write(Value);
                    break;
            }
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

        internal string DebuggerDisplay => Value.ToString();
    }
}
