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

        public static Instruction Read(Stream stream)
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

            switch (opcode)
            {
                case Opcode.Nop:
                    return new Instruction(opcode, default(EmptyOperand));
                default:
                    throw new ArgumentOutOfRangeException("opcode");
            }
        }

        public void Write(Stream stream)
        {
            if ((ushort)Opcode < 256)
            {
                stream.WriteByte((byte)Opcode);
            }
            else
            {
                stream.WriteByte((byte)(((ushort)Opcode & 0xFF00) >> 8));
                stream.WriteByte((byte)((ushort)Opcode & 0xFF));
            }

            if (_operand != null)
            {
                _operand.Write(stream);
            }
        }

        private static class Buffer
        {
            private static ThreadLocal<byte[]> _zero = new ThreadLocal<byte[]>(() => new byte[0]);
            private static ThreadLocal<byte[]> _one = new ThreadLocal<byte[]>(() => new byte[1]);
            private static ThreadLocal<byte[]> _two = new ThreadLocal<byte[]>(() => new byte[2]);
            private static ThreadLocal<byte[]> _four = new ThreadLocal<byte[]>(() => new byte[4]);
            private static ThreadLocal<byte[]> _eight = new ThreadLocal<byte[]>(() => new byte[8]);

            public static byte[] Get(int size)
            {
                switch (size)
                {
                    case 0: return _zero.Value;
                    case 1: return _one.Value;
                    case 2: return _two.Value;
                    case 4: return _four.Value;
                    case 8: return _eight.Value;
                    default: throw new ArgumentOutOfRangeException("size", "Valid values for 'size' are 0, 1, 2, 4, and 8.");
                }
            }
        }
    }
}
