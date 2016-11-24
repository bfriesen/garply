using System;
using System.Diagnostics;
using System.IO;

namespace Garply
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct Instruction
    {
        public const byte MarkerByte1 = 251;
        public const byte MarkerByte2 = 252;
        public const byte MarkerByte3 = 253;
        public const byte MarkerByte4 = 254;
        public const byte MarkerByte5 = 255;
        
        public Instruction(Opcode opcode)
        {
            Opcode = opcode;
            Operand = Value.EmptyOperand;
        }

        public Instruction(Opcode opcode, Value operand)
        {
            Opcode = opcode;
            Operand = operand;
        }

        public Opcode Opcode { get; }
        public Value Operand { get; }

        public static Instruction Read(Stream stream, IMetadataDatabase metadataDatabase)
        {
            Opcode opcode;
            var b = stream.ReadByte();

            switch (b)
            {
                case -1:
                    throw new InvalidOperationException("End of stream");
                case MarkerByte1:
                case MarkerByte2:
                case MarkerByte3:
                case MarkerByte4:
                case MarkerByte5:
                    opcode = (Opcode)(b << 8);
                    b = stream.ReadByte();
                    if (b == -1) throw new InvalidOperationException("End of stream");
                    opcode |= (Opcode)b;
                    break;
                default:
                    opcode = (Opcode)b;
                    break;
            }

            var operandSize = opcode.GetSize();
            var operandData = Buffer.Get(operandSize);
            if (stream.Read(operandData, 0, operandSize) != operandSize) throw new InvalidOperationException("End of stream");

            Value operand;

            switch (opcode)
            {
                case Opcode.Nop:
                case Opcode.PushArg:
                case Opcode.Return:
                case Opcode.GetType:
                case Opcode.TypeIs:
                case Opcode.TupleArity:
                case Opcode.ListEmpty:
                case Opcode.ListAdd:
                //case Opcode.TypeEquals:
                    operand = Value.EmptyOperand;
                    break;
                case Opcode.LoadBoolean:
                    var booleanValue = BitConverter.ToBoolean(operandData, 0);
                    operand = new Value(Boolean.Get(booleanValue), true);
                    break;
                case Opcode.LoadString:
                    var stringId = new Integer(BitConverter.ToInt64(operandData, 0));
                    operand = new Value(metadataDatabase.LoadString(stringId), true);
                    break;
                case Opcode.LoadInteger:
                    var integerValue = BitConverter.ToInt64(operandData, 0);
                    operand = new Value(new Integer(integerValue), true);
                    break;
                case Opcode.LoadFloat:
                    var floatValue = BitConverter.ToDouble(operandData, 0);
                    operand = new Value(new Float(floatValue), true);
                    break;
                case Opcode.LoadType:
                    var type = (Types)BitConverter.ToUInt32(operandData, 0);
                    operand = new Value(TypeValue.Get(type), true);
                    Debug.Assert(operand.Type != Types.Error);
                    break;
                case Opcode.NewTuple:
                    var arity = operandData[0];
                    operand = new Value(new Integer(arity), true);
                    break;
                case Opcode.TupleItem:
                    var index = BitConverter.ToInt64(operandData, 0);
                    operand = new Value(new Integer(index), true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("opcode");
            }

            return new Instruction(opcode, operand);
        }

        public void Write(BinaryWriter writer, IMetadataDatabase metadataDatabase)
        {
            if ((ushort)Opcode < 256)
            {
                writer.Write((byte)Opcode);
            }
            else
            {
                writer.Write((byte)(((ushort)Opcode & 0xFF00) >> 8));
                writer.Write((byte)((ushort)Opcode & 0xFF));
            }

            Operand.Write(writer, metadataDatabase);
        }

        internal string DebuggerDisplay
        {
            get
            {
                string operand;
                switch (Operand.Type & ~Types.Operand)
                {
                    case Types.Boolean: operand = Operand.AsBoolean.DebuggerDisplay; break;
                    case Types.Error: operand = "error"; break;
                    case Types.Float: operand = Operand.AsFloat.DebuggerDisplay; break;
                    case Types.Integer: operand = Operand.AsInteger.DebuggerDisplay; break;
                    case Types.List: operand = Operand.AsList.DebuggerDisplay; break;
                    case Types.String: operand = Operand.AsString.DebuggerDisplay; break;
                    case Types.Tuple: operand = Operand.AsTuple.DebuggerDisplay; break;
                    case Types.Type: operand = Operand.AsType.DebuggerDisplay; break;
                    default: return "Unknown Operand encountered in Instruction.DebuggerDisplay";
                }
                return $"{Opcode}:{operand}";
            }
        }
    }
}
