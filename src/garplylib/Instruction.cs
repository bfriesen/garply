using System;
using System.Diagnostics;
using System.IO;

namespace garply
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
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
                case Opcode.PushArg:
                case Opcode.Return:
                case Opcode.GetType:
                case Opcode.TypeName:
                case Opcode.TypeBaseType:
                case Opcode.TypeIs:
                case Opcode.TupleArity:
                case Opcode.ListEmpty:
                case Opcode.ListAdd:
                //case Opcode.TypeName:
                //case Opcode.TypeBaseType:
                //case Opcode.TypeIs:
                //case Opcode.TypeEquals:
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
                case Opcode.NewTuple:
                    var arity = operandData[0];
                    operand = new Integer(arity);
                    break;
                case Opcode.TupleItem:
                    var index = BitConverter.ToInt64(operandData, 0);
                    operand = new Integer(index);
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

        internal string DebuggerDisplay
        {
            get
            {
                string operand;

                var firstClass = Operand as IFirstClassType;
                if (firstClass != null)
                {
                    if (firstClass.Type.Equals(Types.Empty))
                    {
                        operand = "empty";
                    }
                    else if (firstClass.Type.Equals(Types.Error))
                    {
                        operand = "error";
                    }
                    else
                    {
                        operand = Operand.ToString();
                    }
                }
                else if (Operand is EmptyOperand)
                {
                    operand = "empty";
                }
                else if (Operand is String)
                {
                    operand = ((String)Operand).DebuggerDisplay;
                }
                else if (Operand is Boolean)
                {
                    operand = ((Boolean)Operand).DebuggerDisplay;
                }
                else if (Operand is Float)
                {
                    operand = ((Float)Operand).DebuggerDisplay;
                }
                else if (Operand is Integer)
                {
                    operand = ((Integer)Operand).DebuggerDisplay;
                }
                else if (Operand is Tuple)
                {
                    operand = ((Tuple)Operand).DebuggerDisplay;
                }
                else if (Operand is List)
                {
                    operand = ((List)Operand).DebuggerDisplay;
                }
                else if (Operand is Type)
                {
                    operand = ((Type)Operand).DebuggerDisplay;
                }
                else
                {
                    operand = Operand.ToString();
                }

                return $"{Opcode}:{operand}";
            }
        }
    }
}
