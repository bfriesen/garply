using System;
using System.IO;
using System.Threading;

namespace garply
{
    public struct Instruction
    {
        public const byte MarkerByte1 = 251;
        public const byte MarkerByte2 = 252;
        public const byte MarkerByte3 = 253;
        public const byte MarkerByte4 = 254;
        public const byte MarkerByte5 = 255;
        
        private IOperand _operand;

        public Instruction(Opcode opcode, IOperand operand = null)
        {
            switch (opcode)
            {
                case Opcode.Nop:
                case Opcode.GetType:
                case Opcode.TypeName:
                case Opcode.TypeBaseType:
                case Opcode.TypeIs:
                case Opcode.TypeEquals:
                case Opcode.LoadBoolean:
                case Opcode.LoadString:
                case Opcode.LoadInteger:
                case Opcode.LoadFloat:
                case Opcode.LoadType:
                case Opcode.Return:
                    break;
                case Opcode.Reserved1:
                case Opcode.Reserved2:
                case Opcode.Reserved3:
                case Opcode.Reserved4:
                case Opcode.Reserved5:
                    throw new ArgumentException($"Cannot directly use reserved opcode value: {opcode}", "opcode");
                default:
                    throw new ArgumentOutOfRangeException("opcode", $"Invalid opcode value: {opcode}");
            }

            Opcode = opcode;
            _operand = operand ?? default(EmptyOperand);
        }

        public Opcode Opcode { get; }
        public IOperand Operand { get { return _operand ?? (_operand = default(EmptyOperand)); } }

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

            IOperand operand;

            switch (opcode)
            {
                case Opcode.Nop:
                case Opcode.Return:
                case Opcode.GetType:
                case Opcode.TypeName:
                case Opcode.TypeBaseType:
                case Opcode.TypeIs:
                case Opcode.TypeEquals:
                    operand = default(EmptyOperand);
                    break;
                case Opcode.LoadBoolean:
                    var booleanValue = BitConverter.ToBoolean(operandData, 0);
                    operand = new Boolean(booleanValue);
                    break;
                case Opcode.LoadString:
                    var stringId = new Integer(BitConverter.ToInt64(operandData, 0));
                    operand = metadataDatabase.LoadString(stringId);
                    break;
                case Opcode.LoadInteger:
                    var integerValue = BitConverter.ToInt64(operandData, 0);
                    operand = new Integer(integerValue);
                    break;
                case Opcode.LoadFloat:
                    var floatValue = BitConverter.ToDouble(operandData, 0);
                    operand = new Float(floatValue);
                    break;
                case Opcode.LoadType:
                    var typeId = new Integer(BitConverter.ToInt64(operandData, 0));
                    operand = metadataDatabase.LoadType(typeId) as IOperand;
#if UNSTABLE
                    if (operand == null) throw new InvalidOperationException("Loaded");
#endif
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

            if (_operand != null)
            {
                _operand.Write(writer, metadataDatabase);
            }
        }
    }
}
