using System;
using System.Diagnostics;
using System.IO;

namespace Garply
{
    internal struct Value : IEquatable<Value>
    {
        private const byte _true = 1;
        private const byte _false = 0;

        public readonly long Raw;
        public readonly Types Type;

        internal Value(Types type, long rawValue)
        {
            Type = type;
            Raw = rawValue;
        }

        public Value(bool value)
        {
            Type = Types.Boolean;
            Raw = value ? 1 : 0;
        }

        public Value(long value)
        {
            Type = Types.Integer;
            Raw = value;
        }

        public Value(double value)
        {
            Type = Types.Float;
            Raw = BitConverter.DoubleToInt64Bits(value);
        }

        public Value(Types value)
        {
            Type = Types.Type;
            Raw = (uint)value;
        }

        public void Write(Opcode opcode, BinaryWriter writer)
        {
            switch (Type)
            {
                case Types.Error: break;
                case Types.Boolean: writer.Write(Raw == 0 ? _false : _true); break;
                case Types.Float: writer.Write(BitConverter.Int64BitsToDouble(Raw)); break;
                case Types.Integer:
                    {
                        switch (opcode)
                        {
                            case Opcode.TupleItem:
                                writer.Write((byte)Raw);
                                break;
                            default:
                                writer.Write(Raw);
                                break;
                        }
                        break;
                    }
                case Types.Type: writer.Write((uint)Raw); break;
                case Types.String:
                case Types.Tuple:
                case Types.List:
                case Types.Expression: writer.Write(Raw); break;

                default: Debug.Fail($"Unknown/unwritable type: {Type}"); break;
            }
        }

        public override string ToString()
        {
            switch (Type)
            {
                case Types.Error: return "{Error}";
                case Types.Boolean: return Raw == 0 ? "false" : "true";
                case Types.Float: return BitConverter.Int64BitsToDouble(Raw).ToString();
                case Types.Integer: return Raw.ToString();
                case Types.String: return $@"""{Heap.GetString((int)Raw).Replace(@"""", @"""""")}""";
                case Types.Tuple: return Heap.GetTuple((int)Raw).ToString();
                case Types.List: return Heap.GetList((int)Raw).ToString();
                case Types.Type: return $"<{((Types)(uint)Raw).ToString()}>";
                case Types.Expression: return Heap.GetExpression((int)Raw).ToString();
                default: return $"Unknown Type: {Type}";
            }
        }

        public override int GetHashCode() => unchecked((Raw.GetHashCode() * 397) ^ Type.GetHashCode());
        public override bool Equals(object obj) => obj is Value && Equals((Value)obj);
        public bool Equals(Value other) => Raw == other.Raw && Type == other.Type;
        public static bool operator ==(Value lhs, Value rhs) => lhs.Equals(rhs);
        public static bool operator !=(Value lhs, Value rhs) => !lhs.Equals(rhs);
    }
}
