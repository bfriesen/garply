using System;
using System.Diagnostics;
using System.IO;

namespace Garply
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct Value
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

        public void Write(Opcode opcode, BinaryWriter writer, IMetadataDatabase metadataDatabase)
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
                case Types.List: writer.Write(Raw); break;
                case Types.String: writer.Write(metadataDatabase.GetStringId(Heap.GetString((int)Raw))); break;
                case Types.Tuple: writer.Write(Raw); break;
                case Types.Type: writer.Write((uint)Raw); break;
                default: Debug.Fail($"Unknown/unwritable type: {Type}"); break;
            }
        }

        internal string DebuggerDisplay
        {
            get
            {
                if (Type == Types.Error) return "<Empty>";
                string valueString;
                switch (Type)
                {
                    case Types.Boolean: valueString = (Raw != 0).ToString(); break;
                    case Types.Float: valueString = BitConverter.Int64BitsToDouble(Raw).ToString(); break;
                    case Types.Integer: valueString = Raw.ToString(); break;
                    case Types.String: valueString = Heap.GetString((int)Raw); break;
                    case Types.Tuple: valueString = Heap.GetTuple((int)Raw).DebuggerDisplay; break;
                    case Types.List: valueString = Heap.GetList((int)Raw).DebuggerDisplay; break;
                    case Types.Type: valueString = ((Types)(uint)Raw).ToString(); break;
                    default: return $"Unknown Type: {Type}";
                }
                return $"({Type}) {valueString}";
            }
        }
    }
}
