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
            Type = Types.@bool;
            Raw = value ? 1 : 0;
        }

        public Value(long value)
        {
            Type = Types.@int;
            Raw = value;
        }

        public Value(double value)
        {
            Type = Types.@float;
            Raw = BitConverter.DoubleToInt64Bits(value);
        }

        public Value(Types value)
        {
            Type = Types.type;
            Raw = (uint)value;
        }

        public Value(Opcode value)
        {
            Type = Types.opcode;
            Raw = (uint)value;
        }

        public void Write(Opcode opcode, BinaryWriter writer)
        {
            switch (Type)
            {
                case Types.error: break;
                case Types.@bool: writer.Write(Raw == 0 ? _false : _true); break;
                case Types.@float: writer.Write(BitConverter.Int64BitsToDouble(Raw)); break;
                case Types.@int:
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
                case Types.type: writer.Write((uint)Raw); break;
                case Types.@string:
                case Types.tuple:
                case Types.list:
                case Types.expression: writer.Write(Raw); break;

                default: Debug.Fail($"Unknown/unwritable type: {Type}"); break;
            }
        }

        public override string ToString()
        {
            switch (Type)
            {
                case Types.error: return "#Error#";
                case Types.@bool: return Raw == 0 ? "false" : "true";
                case Types.@float: return BitConverter.Int64BitsToDouble(Raw).ToString();
                case Types.@int: return Raw.ToString();
                case Types.@string: return $@"""{Heap.GetString((int)Raw).Replace(@"""", @"""""")}""";
                case Types.tuple: return Heap.GetTuple((int)Raw).ToString();
                case Types.list: return Heap.GetList((int)Raw).ToString();
                case Types.type: return $"<{((Types)(uint)Raw).ToString()}>";
                case Types.opcode: return $"|{((Opcode)(ushort)Raw).ToString()}|";
                case Types.expression: return Heap.GetExpression((int)Raw).ToString();
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
