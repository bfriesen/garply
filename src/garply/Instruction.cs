using System;
using System.IO;

namespace Garply
{
    internal partial struct Instruction
    {
        public const byte MarkerByte1 = 251;
        public const byte MarkerByte2 = 252;
        public const byte MarkerByte3 = 253;
        public const byte MarkerByte4 = 254;
        public const byte MarkerByte5 = 255;

        private Instruction(Opcode opcode)
        {
            Opcode = opcode;
            Operand = default(Value);
        }

        private Instruction(Opcode opcode, Value operand)
        {
            Opcode = opcode;
            Operand = operand;
        }

        public Opcode Opcode { get; }
        public Value Operand { get; }

        public static Instruction Read(Stream stream)
        {
            var opcode = ReadOpcode(stream);
            var operandData = ReadOperandData(stream, opcode);
            var operand = GetOperand(opcode, operandData);
            return new Instruction(opcode, operand);
        }

        private static Value GetOperand(Opcode opcode, byte[] operandData)
        {
            Value operand;
            switch (opcode)
            {
                case Opcode.Nop:
                case Opcode.GetType:
                case Opcode.TypeIs:
                case Opcode.TypeEquals:
                case Opcode.TupleArity:
                case Opcode.ListEmpty:
                case Opcode.ListAdd:
                case Opcode.ListHead:
                case Opcode.ListTail:
                    operand = default(Value);
                    break;
                case Opcode.LoadBoolean:
                    var booleanValue = BitConverter.ToBoolean(operandData, 0);
                    operand = new Value(booleanValue);
                    break;
                case Opcode.LoadInteger:
                    var integerValue = BitConverter.ToInt64(operandData, 0);
                    operand = new Value(integerValue);
                    break;
                case Opcode.LoadFloat:
                    var floatValue = BitConverter.ToDouble(operandData, 0);
                    operand = new Value(floatValue);
                    break;
                case Opcode.LoadType:
                    var type = (Types)BitConverter.ToUInt32(operandData, 0);
                    operand = new Value(type);
                    break;
                case Opcode.LoadString:
                    var stringId = BitConverter.ToInt32(operandData, 0);
                    operand = new Value(Types.String, stringId);
                    break;
                case Opcode.NewTuple:
                    var arity = operandData[0];
                    operand = new Value(arity);
                    break;
                case Opcode.TupleItem:
                    var index = operandData[0];
                    operand = new Value(index);
                    break;
                case Opcode.AssignVariable:
                case Opcode.ReadVariable:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException("opcode");
            }

            return operand;
        }

        private static byte[] ReadOperandData(Stream stream, Opcode opcode)
        {
            var operandSize = opcode.GetSize();
            var operandData = Buffer.Get(operandSize);
            if (stream.Read(operandData, 0, operandSize) != operandSize) throw new InvalidOperationException("End of stream");
            return operandData;
        }

        private static Opcode ReadOpcode(Stream stream)
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

            return opcode;
        }

        public void Write(BinaryWriter writer)
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

            Operand.Write(Opcode, writer);
        }

        public override string ToString()
        {
            if (Operand.Type == Types.Error)
            {
                return Opcode.ToString();
            }
            return $"{Opcode}:{Operand.ToString()}";
        }
    }
}
